using Common.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QuizSystem.BLL.Dtos.Auth;
using QuizSystem.BLL.Interfaces;
using QuizSystem.Common.Common;
using QuizSystem.Common.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuizSystem.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager,
                           IUnitOfWork unitOfWork,
                           IConfiguration configuration)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return Result.Fail<AuthResponseDto>("Email is already registered.", ErrorType.Conflict);

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Fail<AuthResponseDto>(errors, ErrorType.Validation);
            }

            Guid profileId;
            string role;

            switch (dto.UserType)
            {
                case UserType.Instructor:
                    role = AppRoles.Instructor;
                    await _userManager.AddToRoleAsync(user, role);

                    var instructor = new Instructor { FullName = dto.FullName, UserId = user.Id };
                    await _unitOfWork.InstructorRepository.AddAsync(instructor);
                    await _unitOfWork.CompleteAsync();
                    profileId = instructor.Id;
                    break;

                case UserType.Student:
                    role = AppRoles.Student;
                    await _userManager.AddToRoleAsync(user, role);

                    var student = new Student { FullName = dto.FullName, UserId = user.Id };
                    await _unitOfWork.StudentRepository.AddAsync(student);
                    await _unitOfWork.CompleteAsync();
                    profileId = student.Id;
                    break;

                default:
                    return Result.Fail<AuthResponseDto>("Invalid user type.", ErrorType.Validation);
            }

            var token = GenerateJwtToken(user, role, profileId);

            return Result.Success(new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                FullName = dto.FullName,
                Role = role,
                ProfileId = profileId
            });
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Result.Fail<AuthResponseDto>("Invalid email or password.", ErrorType.Unauthorized);

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? AppRoles.Student;

            // تحسين: فصل منطق جلب البروفايل
            var (profileId, fullName) = await GetUserProfileAsync(user.Id, role);

            if (profileId == Guid.Empty)
            {
                // حالة حرجة: المستخدم موجود لكن ليس له بروفايل (بيانات غير متسقة)
                return Result.Fail<AuthResponseDto>("User profile data is missing or corrupted.", ErrorType.Failure);
            }

            var token = GenerateJwtToken(user, role, profileId);

            return Result.Success(new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                FullName = fullName,
                Role = role,
                ProfileId = profileId
            });
        }

        // --- Helper: Get User Profile ---
        private async Task<(Guid Id, string Name)> GetUserProfileAsync(string userId, string role)
        {
            if (role == AppRoles.Instructor)
            {
                var instructors = await _unitOfWork.InstructorRepository.FindAsync(i => i.UserId == userId);
                var instructor = instructors.FirstOrDefault();
                return instructor != null ? (instructor.Id, instructor.FullName) : (Guid.Empty, string.Empty);
            }
            else if (role == AppRoles.Student)
            {
                var students = await _unitOfWork.StudentRepository.FindAsync(s => s.UserId == userId);
                var student = students.FirstOrDefault();
                return student != null ? (student.Id, student.FullName) : (Guid.Empty, string.Empty);
            }

            return (Guid.Empty, string.Empty);
        }

        // --- Helper: JWT Generation ---
        private string GenerateJwtToken(ApplicationUser user, string role, Guid profileId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, role)
            };

            if (role == AppRoles.Instructor)
            {
                claims.Add(new Claim(CustomClaimTypes.InstructorId, profileId.ToString()));
            }
            else if (role == AppRoles.Student)
            {
                claims.Add(new Claim(CustomClaimTypes.StudentId, profileId.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["JWT:DurationInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}