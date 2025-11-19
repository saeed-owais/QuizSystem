using Microsoft.AspNetCore.Mvc;
using QuizSystem.BLL.Dtos.Auth;
using QuizSystem.BLL.Interfaces;

namespace QuizSystem.API.Controllers
{
    public class AuthController : ApiClientBaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            if (!result.IsSuccess)
            {
                return HandleErrorResult(result);
            }

            return Ok(result.Data);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (!result.IsSuccess)
            {
                return HandleErrorResult(result);
            }

            return Ok(result.Data);
        }
    }
}