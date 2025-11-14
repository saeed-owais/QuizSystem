using QuizSystem.Common.Common;

namespace Common.Entities
{
    public class Student : BaseEntity
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
    }
}