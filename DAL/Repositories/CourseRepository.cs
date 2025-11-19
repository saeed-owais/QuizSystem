using Common.Entities;
using Microsoft.EntityFrameworkCore;
using QuizSystem.BLL.Interfaces;
using QuizSystem.DAL.Data;
using QuizSystem.DAL.Repositories;

namespace DAL.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<Course> GetCourseWithInstructorAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new Course
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    InstructorId = c.InstructorId,
                    Instructor = new Instructor
                    {
                        FullName = c.Instructor.FullName
                    }
                })
                .FirstOrDefaultAsync(ct);
        }
    }
}
