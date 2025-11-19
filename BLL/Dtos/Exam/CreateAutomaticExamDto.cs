using Common.Enums;

namespace QuizSystem.BLL.Dtos.Exam
{
    public class CreateAutomaticExamDto
    {
        public string Title { get; set; }
        public ExamType ExamType { get; set; }
        public Guid CourseId { get; set; }

        // قائمة المعايير (مثلاً: 3 سهلة، 2 صعبة)
        public List<ExamGenerationCriteriaDto> Criteria { get; set; }
    }
}