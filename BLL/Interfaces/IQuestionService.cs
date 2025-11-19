using QuizSystem.BLL.Dtos.Question;
using QuizSystem.Common.Common;

namespace QuizSystem.BLL.Interfaces
{
    public interface IQuestionService
    {
        Task<Result<QuestionDto>> CreateQuestionAsync(CreateQuestionDto dto, Guid instructorId, CancellationToken ct = default);
        Task<Result<QuestionDto>> GetQuestionByIdAsync(Guid id, CancellationToken ct = default);
        Task<Result<IEnumerable<QuestionDto>>> GetQuestionsByInstructorAsync(Guid instructorId, CancellationToken ct = default);
        Task<Result> UpdateQuestionAsync(Guid id, UpdateQuestionDto dto, Guid instructorId, CancellationToken ct = default);
        Task<Result> DeleteQuestionAsync(Guid id, Guid instructorId, CancellationToken ct = default);

    }
}