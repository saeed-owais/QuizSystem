using QuizSystem.Common.Common;

namespace Common.Entities
{
    public class ExamQuestion : BaseEntity
    {
        public Guid ExamId { get; set; }
        public Exam Exam { get; set; }
        public Guid QuestionId { get; set; }
        public Question Question { get; set; }
    }
}