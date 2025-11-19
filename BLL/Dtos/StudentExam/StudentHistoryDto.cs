namespace QuizSystem.BLL.Dtos.StudentExam
{
    public class StudentHistoryDto
    {
        public Guid ExamId { get; set; }
        public string ExamTitle { get; set; }
        public string CourseName { get; set; }
        public double Score { get; set; }
        public DateTime SubmittedDate { get; set; }
    }
}