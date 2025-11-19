using QuizSystem.BLL.Dtos.StudentExam;
using QuizSystem.Common.Common;

namespace QuizSystem.BLL.Interfaces
{
    public interface IStudentExamService
    {
        // بدء الامتحان (جلب الورقة)
        Task<Result<List<StudentExamQuestionDto>>> StartExamAsync(Guid examId, Guid studentId, CancellationToken ct = default);

        // تسليم الامتحان (التصحيح)
        Task<Result<ExamResultDto>> SubmitExamAsync(SubmitExamDto dto, Guid studentId, CancellationToken ct = default);
    }
}