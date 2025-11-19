using QuizSystem.BLL.Dtos.Exam;
using QuizSystem.BLL.Dtos.StudentExam;
using QuizSystem.Common.Common;

namespace QuizSystem.BLL.Interfaces
{
    public interface IExamService
    {
        Task<Result<ExamDto>> CreateExamAsync(CreateExamDto dto, Guid instructorId, CancellationToken ct = default);
        Task<Result<ExamDto>> GetExamByIdAsync(Guid id, CancellationToken ct = default);
        Task<Result<IEnumerable<ExamDto>>> GetExamsByCourseAsync(Guid courseId, Guid instructorId, CancellationToken ct = default);

        // إضافة أسئلة يدوياً
        Task<Result> AddQuestionsToExamAsync(Guid examId, List<Guid> questionIds, Guid instructorId, CancellationToken ct = default);

        Task<Result<IEnumerable<ExamResultReportDto>>> GetExamResultsAsync(Guid examId, Guid instructorId, CancellationToken ct = default);
    }
}