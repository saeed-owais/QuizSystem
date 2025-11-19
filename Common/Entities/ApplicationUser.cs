using Common.Entities;
using Microsoft.AspNetCore.Identity;

namespace QuizSystem.Common.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public Instructor Instructor { get; set; }

        public Student Student { get; set; }
    }
}