using FluentValidation;
using QuizSystem.BLL.Dtos.Exam;

namespace QuizSystem.BLL.Validation
{
    public class AddQuestionsToExamDtoValidator : AbstractValidator<AddQuestionsToExamDto>
    {
        public AddQuestionsToExamDtoValidator()
        {
            RuleFor(x => x.QuestionIds)
                .NotEmpty().WithMessage("You must select at least one question.")
                .Must(ids => ids != null && ids.Distinct().Count() == ids.Count)
                .WithMessage("Duplicate questions are not allowed.");
        }
    }
}