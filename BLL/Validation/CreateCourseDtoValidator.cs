using BLL.Dtos.Course;
using FluentValidation;

namespace QuizSystem.BLL.Validation
{
    public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
    {
        public CreateCourseDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Course name is required.")
                .MaximumLength(100).WithMessage("Course name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}