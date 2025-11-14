using Common.Entities;
using Microsoft.AspNetCore.Identity;

namespace QuizSystem.Common.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public Guid? InstructorId { get; set; }
        public Instructor Instructor { get; set; }

        public Guid? StudentId { get; set; }
        public Student Student { get; set; }
    }
}