namespace QuizSystem.BLL.Dtos.Question
{
    public class ChoiceDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
}