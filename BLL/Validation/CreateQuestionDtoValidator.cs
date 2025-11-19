using FluentValidation;
using QuizSystem.BLL.Dtos.Question;

namespace QuizSystem.BLL.Validation
{
    public class CreateQuestionDtoValidator : AbstractValidator<CreateQuestionDto>
    {
        public CreateQuestionDtoValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Question text is required.")
                .MaximumLength(1000);

            RuleFor(x => x.Level)
                .IsInEnum().WithMessage("Invalid question level.");

            RuleFor(x => x.Choices)
                .NotEmpty().WithMessage("A question must have choices.")
                .Must(c => c != null && c.Count >= 2).WithMessage("A question must have at least 2 choices.")
                .Must(c => c != null && c.Count(x => x.IsCorrect) == 1).WithMessage("A question must have exactly one correct answer.");

            RuleForEach(x => x.Choices).SetValidator(new CreateChoiceDtoValidator());
        }
    }

    public class CreateChoiceDtoValidator : AbstractValidator<CreateChoiceDto>
    {
        public CreateChoiceDtoValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Choice text is required.")
                .MaximumLength(500);
        }
    }
}