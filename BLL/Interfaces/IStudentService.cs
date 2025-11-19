using QuizSystem.BLL.Dtos.Student;
using QuizSystem.Common.Common;

namespace QuizSystem.BLL.Interfaces
{
    public interface IStudentService
    {
        Task<Result> EnrollInCourseAsync(Guid studentId, Guid courseId, CancellationToken ct = default);
        Task<Result<IEnumerable<StudentCourseDto>>> GetMyCoursesAsync(Guid studentId, CancellationToken ct = default);
    }
}