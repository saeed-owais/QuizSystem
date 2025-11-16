using BLL.Dtos.Course;
using FluentValidation;

namespace QuizSystem.BLL.Validation
{
    public class UpdateCourseDtoValidator : AbstractValidator<UpdateCourseDto>
    {
        public UpdateCourseDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Course name is required.")
                .MaximumLength(250).WithMessage("Course name cannot exceed 250 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");
        }
    }
}