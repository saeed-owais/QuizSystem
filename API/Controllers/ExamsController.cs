using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.BLL.Dtos.Exam;
using QuizSystem.BLL.Interfaces;
using QuizSystem.Common.Common;

namespace QuizSystem.API.Controllers
{
    [Authorize(Roles = AppRoles.Instructor)]
    public class ExamsController : ApiClientBaseController
    {
        private readonly IExamService _examService;

        public ExamsController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExam([FromBody] CreateExamDto dto, CancellationToken ct)
        {
            var instructorId = GetCurrentInstructorId();
            if (instructorId == Guid.Empty) return Unauthorized();

            var result = await _examService.CreateExamAsync(dto, instructorId, ct);

            if (!result.IsSuccess) return HandleErrorResult(result);

            return CreatedAtAction(nameof(GetExamById), new { id = result.Data.Id }, result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExamById(Guid id, CancellationToken ct)
        {
            var result = await _examService.GetExamByIdAsync(id, ct);
            if (!result.IsSuccess) return HandleErrorResult(result);
            return Ok(result.Data);
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetExamsByCourse(Guid courseId, CancellationToken ct)
        {
            var instructorId = GetCurrentInstructorId();
            if (instructorId == Guid.Empty) return Unauthorized();

            var result = await _examService.GetExamsByCourseAsync(courseId, instructorId, ct);
            if (!result.IsSuccess) return HandleErrorResult(result);
            return Ok(result.Data);
        }

        [HttpPost("{id}/questions")]
        public async Task<IActionResult> AddQuestions(Guid id, [FromBody] AddQuestionsToExamDto dto, CancellationToken ct)
        {
            var instructorId = GetCurrentInstructorId();
            if (instructorId == Guid.Empty) return Unauthorized();

            var result = await _examService.AddQuestionsToExamAsync(id, dto.QuestionIds, instructorId, ct);

            if (!result.IsSuccess) return HandleErrorResult(result);

            return Ok(new { Message = "Questions added successfully." });
        }

        [HttpGet("{id}/results")]
        public async Task<IActionResult> GetExamResults(Guid id, CancellationToken ct)
        {
            var instructorId = GetCurrentInstructorId();
            if (instructorId == Guid.Empty) return Unauthorized();

            var result = await _examService.GetExamResultsAsync(id, instructorId, ct);

            if (!result.IsSuccess) return HandleErrorResult(result);

            return Ok(result.Data);
        }

        [HttpPost("automatic")]
        public async Task<IActionResult> CreateAutomaticExam([FromBody] CreateAutomaticExamDto dto, CancellationToken ct)
        {
            var instructorId = GetCurrentInstructorId();
            if (instructorId == Guid.Empty) return Unauthorized();

            var result = await _examService.CreateAutomaticExamAsync(dto, instructorId, ct);

            if (!result.IsSuccess) return HandleErrorResult(result);

            return CreatedAtAction(nameof(GetExamById), new { id = result.Data.Id }, result.Data);
        }
    }
}