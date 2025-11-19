using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.BLL.Dtos.StudentExam;
using QuizSystem.BLL.Interfaces;
using QuizSystem.Common.Common;
using System.Security.Claims;

namespace QuizSystem.API.Controllers
{
    [Authorize(Roles = AppRoles.Student)] // للطلاب فقط
    public class StudentExamsController : ApiClientBaseController
    {
        private readonly IStudentExamService _studentExamService;

        public StudentExamsController(IStudentExamService studentExamService)
        {
            _studentExamService = studentExamService;
        }

        private Guid GetCurrentStudentId()
        {
            var claim = User.FindFirstValue(CustomClaimTypes.StudentId);
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }

        [HttpGet("{examId}/start")]
        public async Task<IActionResult> StartExam(Guid examId, CancellationToken ct)
        {
            var studentId = GetCurrentStudentId();
            if (studentId == Guid.Empty) return Unauthorized();

            var result = await _studentExamService.StartExamAsync(examId, studentId, ct);

            if (!result.IsSuccess) return HandleErrorResult(result);

            return Ok(result.Data);
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitExam([FromBody] SubmitExamDto dto, CancellationToken ct)
        {
            var studentId = GetCurrentStudentId();
            if (studentId == Guid.Empty) return Unauthorized();

            var result = await _studentExamService.SubmitExamAsync(dto, studentId, ct);

            if (!result.IsSuccess) return HandleErrorResult(result);

            return Ok(result.Data);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetMyHistory(CancellationToken ct)
        {
            var studentId = GetCurrentStudentId();
            if (studentId == Guid.Empty) return Unauthorized();

            var result = await _studentExamService.GetStudentExamHistoryAsync(studentId, ct);

            return Ok(result.Data);
        }
    }
}