using FluentValidation;
using QuizSystem.BLL.Dtos.Auth;

namespace QuizSystem.BLL.Validation
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.UserType).IsInEnum().WithMessage("Invalid user type.");
        }
    }
}