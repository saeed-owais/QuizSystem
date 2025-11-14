using QuizSystem.Common.Common;

namespace Common.Entities
{
    public class StudentCourse : BaseEntity
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    }
}