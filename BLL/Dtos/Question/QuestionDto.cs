using Common.Enums;

namespace QuizSystem.BLL.Dtos.Question
{
    public class QuestionDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public QuestionLevel Level { get; set; }
        public List<ChoiceDto> Choices { get; set; }
    }
}