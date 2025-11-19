using Common.Enums;

namespace QuizSystem.BLL.Dtos.Exam
{
    public class ExamGenerationCriteriaDto
    {
        public QuestionLevel Level { get; set; }
        public int Count { get; set; }
    }
}