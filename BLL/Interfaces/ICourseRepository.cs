using Common.Entities;

namespace QuizSystem.BLL.Interfaces
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<Course> GetCourseWithInstructorAsync(Guid id, CancellationToken ct = default);
    }
}