using Common.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuizSystem.Common.Common;
using QuizSystem.Common.Entities;
using System.Reflection;
using System.Security.Claims;

namespace QuizSystem.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<StudentExam> StudentExams { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaveChanges();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            OnBeforeSaveChanges();
            return base.SaveChanges();
        }

        private void OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var entries = ChangeTracker.Entries<BaseEntity>();

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.CreatedBy = userId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.LastModifiedDate = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = userId;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedDate = DateTime.UtcNow;
                    entry.Entity.DeletedBy = userId;
                }
            }
        }
    }
}