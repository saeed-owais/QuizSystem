using FluentValidation;
using QuizSystem.BLL.Dtos.Exam;

namespace QuizSystem.BLL.Validation
{
    public class CreateAutomaticExamDtoValidator : AbstractValidator<CreateAutomaticExamDto>
    {
        public CreateAutomaticExamDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(250);
            RuleFor(x => x.ExamType).IsInEnum();
            RuleFor(x => x.CourseId).NotEmpty();

            RuleFor(x => x.Criteria)
                .NotEmpty().WithMessage("You must specify generation criteria.")
                .Must(c => c != null && c.Sum(x => x.Count) > 0)
                .WithMessage("Total number of questions must be greater than zero.");

            // التحقق من أن العدد المطلوب لكل مستوى منطقي (أكبر من صفر)
            RuleForEach(x => x.Criteria).ChildRules(criteria =>
            {
                criteria.RuleFor(x => x.Level).IsInEnum();
                criteria.RuleFor(x => x.Count).GreaterThan(0).WithMessage("Count for each level must be greater than 0.");
            });
        }
    }
}