using Common.Enums;

namespace QuizSystem.BLL.Dtos.Question
{
    public class UpdateQuestionDto
    {
        public string Text { get; set; }
        public QuestionLevel Level { get; set; }
        public List<CreateChoiceDto> Choices { get; set; }
    }
}