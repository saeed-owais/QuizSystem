using Common.Enums;
using QuizSystem.Common.Common;

namespace Common.Entities
{
    public class Exam : BaseEntity
    {
        public string Title { get; set; }
        public ExamType ExamType { get; set; }
        public int NumberOfQuestions { get; set; }
        public bool IsAutomatic { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
        public ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
    }
}