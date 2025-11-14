using Common.Enums;
using QuizSystem.Common.Common;

namespace Common.Entities
{
    public class Question : BaseEntity
    {
        public string Text { get; set; }
        public QuestionLevel Level { get; set; }
        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; }
        public ICollection<Choice> Choices { get; set; } = new List<Choice>();
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
    }
}