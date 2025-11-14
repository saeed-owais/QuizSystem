using QuizSystem.Common.Common;

namespace Common.Entities
{
    public class StudentExam : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Student Student { get; set; }
        public Guid ExamId { get; set; }
        public Exam Exam { get; set; }
        public double? Score { get; set; }
        public DateTime? SubmittedDate { get; set; }
    }
}