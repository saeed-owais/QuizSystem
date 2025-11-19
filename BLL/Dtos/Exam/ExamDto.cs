using Common.Enums;

namespace QuizSystem.BLL.Dtos.Exam
{
    public class ExamDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public ExamType ExamType { get; set; }
        public int NumberOfQuestions { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        // يمكن إضافة قائمة الأسئلة هنا إذا أردنا عرضها مع الامتحان
        // public List<QuestionDto> Questions { get; set; } 
    }
}