namespace QuizSystem.BLL.Dtos.StudentExam
{
    public class StudentExamQuestionDto
    {
        public Guid Id { get; set; } // QuestionId
        public string Text { get; set; }
        public List<StudentExamChoiceDto> Choices { get; set; }
    }
}