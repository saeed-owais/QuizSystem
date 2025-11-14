using QuizSystem.Common.Common;

namespace Common.Entities
{
    public class ExamQuestion : BaseEntity
    {
        public string ExamId { get; set; }
        public Exam Exam { get; set; }
        public string QuestionId { get; set; }
        public Question Question { get; set; }
    }
}