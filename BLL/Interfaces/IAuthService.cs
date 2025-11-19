using QuizSystem.BLL.Dtos.Auth;
using QuizSystem.Common.Common;

namespace QuizSystem.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto);
        Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto);
    }
}