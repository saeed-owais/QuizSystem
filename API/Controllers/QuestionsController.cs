using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.BLL.Dtos.Question;
using QuizSystem.BLL.Interfaces;
using QuizSystem.Common.Common;

namespace QuizSystem.API.Controllers
{
    [Authorize(Roles = AppRoles.Instructor)]
    public class QuestionsController : ApiClientBaseController
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionDto dto, CancellationToken ct)
        {
            var instructorId = GetCurrentInstructorId();
            if (instructorId == Guid.Empty) return Unauthorized();

            var result = await _questionService.CreateQuestionAsync(dto, instructorId, ct);

            if (!result.IsSuccess)
            {
                return HandleErrorResult(result);
            }

            return CreatedAtAction(nameof(GetQuestionById), new { id = result.Data.Id }, result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionById(Guid id, CancellationToken ct)
        {
            var result = await _questionService.GetQuestionByIdAsync(id, ct);

            if (!result.IsSuccess)
            {
                return HandleErrorResult(result);
            }

            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyQuestions(CancellationToken ct)
        {
            var instructorId = GetCurrentInstructorId();
            if (instructorId == Guid.Empty) return Unauthorized();

            var result = await _questionService.GetQuestionsByInstructorAsync(instructorId, ct);

            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(Guid id, [FromBody] UpdateQuestionDto dto, CancellationToken ct)
        {
            var instructorId = GetCurrentInstructorId();
            if (instructorId == Guid.Empty) return Unauthorized();

            var result = await _questionService.UpdateQuestionAsync(id, dto, instructorId, ct);

            if (!result.IsSuccess)
            {
                return HandleErrorResult(result);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(Guid id, CancellationToken ct)
        {
            var instructorId = GetCurrentInstructorId();
            if (instructorId == Guid.Empty) return Unauthorized();

            var result = await _questionService.DeleteQuestionAsync(id, instructorId, ct);

            if (!result.IsSuccess)
            {
                return HandleErrorResult(result);
            }

            return NoContent();
        }
    }
}