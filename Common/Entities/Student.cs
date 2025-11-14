using QuizSystem.Common.Common;
using QuizSystem.Common.Entities;

namespace Common.Entities
{
    public class Student : BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string FullName { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
    }
}