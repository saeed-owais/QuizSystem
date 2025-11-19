namespace QuizSystem.BLL.Dtos.StudentExam
{
    public class StudentExamChoiceDto
    {
        public Guid Id { get; set; } // ChoiceId
        public string Text { get; set; }
        // ⚠️ هام جداً: لا تضع IsCorrect هنا
    }
}