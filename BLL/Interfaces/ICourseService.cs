using QuizSystem.BLL.Dtos.Course;

namespace QuizSystem.BLL.Interfaces
{
    public interface ICourseService
    {
        Task<CourseDto> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default);
        Task<IEnumerable<CourseDto>> GetCoursesByInstructorAsync(Guid instructorId, CancellationToken cancellationToken = default);
        Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto, Guid instructorId, CancellationToken cancellationToken = default);
    }
}