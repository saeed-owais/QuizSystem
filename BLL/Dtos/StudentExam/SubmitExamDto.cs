namespace QuizSystem.BLL.Dtos.StudentExam
{
    public class SubmitExamDto
    {
        public Guid ExamId { get; set; }
        public List<StudentAnswerDto> Answers { get; set; }
    }

    public class StudentAnswerDto
    {
        public Guid QuestionId { get; set; }
        public Guid ChoiceId { get; set; }
    }
}