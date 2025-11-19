using FluentValidation;
using QuizSystem.BLL.Dtos.Exam;

namespace QuizSystem.BLL.Validation
{
    public class CreateExamDtoValidator : AbstractValidator<CreateExamDto>
    {
        public CreateExamDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(250);
            RuleFor(x => x.ExamType).IsInEnum();
            RuleFor(x => x.CourseId).NotEmpty();
        }
    }
}