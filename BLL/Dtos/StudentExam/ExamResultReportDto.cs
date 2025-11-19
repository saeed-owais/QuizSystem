namespace QuizSystem.BLL.Dtos.StudentExam
{
    public class ExamResultReportDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public double Score { get; set; }
        public DateTime SubmittedDate { get; set; }
    }
}