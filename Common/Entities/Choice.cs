using QuizSystem.Common.Common;

namespace Common.Entities
{
    public class Choice : BaseEntity
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public string QuestionId { get; set; }
        public Question Question { get; set; }
    }
}