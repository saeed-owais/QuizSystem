using Common.Enums;

namespace QuizSystem.BLL.Dtos.Exam
{
    public class CreateExamDto
    {
        public string Title { get; set; }
        public ExamType ExamType { get; set; }
        public Guid CourseId { get; set; }
    }
}