using AutoMapper;
using Common.Entities;
using QuizSystem.BLL.Dtos.Question;
using QuizSystem.BLL.Interfaces;
using QuizSystem.Common.Common;

namespace QuizSystem.BLL.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QuestionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<QuestionDto>> CreateQuestionAsync(CreateQuestionDto dto, Guid instructorId, CancellationToken ct = default)
        {
            var question = _mapper.Map<Question>(dto);

            question.InstructorId = instructorId;

            await _unitOfWork.QuestionRepository.AddAsync(question, ct);

            await _unitOfWork.CompleteAsync(ct);

            return Result.Success(_mapper.Map<QuestionDto>(question));
        }

        public async Task<Result<QuestionDto>> GetQuestionByIdAsync(Guid id, CancellationToken ct = default)
        {
            var questionsQuery = await _unitOfWork.QuestionRepository.FindAsync(q => q.Id == id, ct);
            var question = questionsQuery.FirstOrDefault();

            if (question == null)
                return Result.Fail<QuestionDto>("Question not found.");

            // Manually loading choices (because GenericRepo doesn't support Include)
            var choices = await _unitOfWork.ChoiceRepository.FindAsync(c => c.QuestionId == id, ct);
            question.Choices = choices.ToList();

            return Result.Success(_mapper.Map<QuestionDto>(question));
        }

        public async Task<Result<IEnumerable<QuestionDto>>> GetQuestionsByInstructorAsync(Guid instructorId, CancellationToken ct = default)
        {
            var questions = await _unitOfWork.QuestionRepository.FindAsync(q => q.InstructorId == instructorId, ct);

            return Result.Success(_mapper.Map<IEnumerable<QuestionDto>>(questions));
        }

        public async Task<Result> UpdateQuestionAsync(Guid id, UpdateQuestionDto dto, Guid instructorId, CancellationToken ct = default)
        {
            var question = await _unitOfWork.QuestionRepository.GetByIdAsync(id, ct);

            if (question == null)
                return Result.Fail("Question not found.", ErrorType.NotFound);

            if (question.InstructorId != instructorId)
                return Result.Fail("You are not authorized to update this question.", ErrorType.Unauthorized);

            _mapper.Map(dto, question);

            // 2. Handling Choices (Full Replacement)
            // A. Fetch old choices
            var existingChoices = await _unitOfWork.ChoiceRepository.FindAsync(c => c.QuestionId == id, ct);

            // B. Delete old choices
            foreach (var choice in existingChoices)
            {
                _unitOfWork.ChoiceRepository.Delete(choice);
            }

            // C. Add new choices
            foreach (var choiceDto in dto.Choices)
            {
                var newChoice = _mapper.Map<Choice>(choiceDto);
                newChoice.QuestionId = id;
                await _unitOfWork.ChoiceRepository.AddAsync(newChoice, ct);
            }

            // 3. Save Changes (Update Question + Delete Old Choices + Insert New Choices)
            // All this happens in a single Transaction automatically thanks to UnitOfWork
            _unitOfWork.QuestionRepository.Update(question);
            await _unitOfWork.CompleteAsync(ct);

            return Result.Success();
        }

        public async Task<Result> DeleteQuestionAsync(Guid id, Guid instructorId, CancellationToken ct = default)
        {
            var question = await _unitOfWork.QuestionRepository.GetByIdAsync(id, ct);

            if (question == null)
                return Result.Fail("Question not found.", ErrorType.NotFound);

            if (question.InstructorId != instructorId)
                return Result.Fail("You are not authorized to delete this question.", ErrorType.Unauthorized);

            _unitOfWork.QuestionRepository.Delete(question);
            await _unitOfWork.CompleteAsync(ct);

            return Result.Success();
        }
    }
}