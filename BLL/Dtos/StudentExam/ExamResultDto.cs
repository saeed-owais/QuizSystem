namespace QuizSystem.BLL.Dtos.StudentExam
{
    public class ExamResultDto
    {
        public double Score { get; set; } // الدرجة المئوية
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
    }
}