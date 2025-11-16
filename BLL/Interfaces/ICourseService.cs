using BLL.Dtos.Course;
using QuizSystem.Common.Common;

namespace QuizSystem.BLL.Interfaces
{
    public interface ICourseService
    {
        Task<Result<CourseDto>> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<CourseDto>>> GetCoursesByInstructorAsync(Guid instructorId, CancellationToken cancellationToken = default);
        Task<Result<CourseDto>> CreateCourseAsync(CreateCourseDto createCourseDto, Guid instructorId, CancellationToken cancellationToken = default);
        Task<Result> UpdateCourseAsync(Guid courseId, UpdateCourseDto updateCourseDto, Guid instructorId, CancellationToken cancellationToken = default);
        Task<Result> DeleteCourseAsync(Guid courseId, Guid instructorId, CancellationToken cancellationToken = default);
    }
}