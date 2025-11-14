using QuizSystem.Common.Common;

namespace Common.Entities
{
    public class Instructor : BaseEntity
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}