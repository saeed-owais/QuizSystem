using FluentValidation;
using QuizSystem.BLL.Dtos.Question;

namespace QuizSystem.BLL.Validation
{
    public class UpdateQuestionDtoValidator : AbstractValidator<UpdateQuestionDto>
    {
        public UpdateQuestionDtoValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Question text is required.")
                .MaximumLength(1000);

            RuleFor(x => x.Level)
                .IsInEnum();

            RuleFor(x => x.Choices)
                .NotEmpty().WithMessage("A question must have choices.")
                .Must(c => c != null && c.Count >= 2).WithMessage("A question must have at least 2 choices.")
                .Must(c => c != null && c.Count(x => x.IsCorrect) == 1).WithMessage("A question must have exactly one correct answer.");

            RuleForEach(x => x.Choices).SetValidator(new CreateChoiceDtoValidator());
        }
    }
}