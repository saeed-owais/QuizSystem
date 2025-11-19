using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.BLL.Interfaces;
using QuizSystem.Common.Common;
using System.Security.Claims;

namespace QuizSystem.API.Controllers
{
    [Authorize(Roles = AppRoles.Student)] // للطلاب فقط
    public class StudentsController : ApiClientBaseController
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        private Guid GetCurrentStudentId()
        {
            var claim = User.FindFirstValue(CustomClaimTypes.StudentId);
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }

        [HttpPost("enroll/{courseId}")]
        public async Task<IActionResult> EnrollInCourse(Guid courseId, CancellationToken ct)
        {
            var studentId = GetCurrentStudentId();
            if (studentId == Guid.Empty) return Unauthorized();

            var result = await _studentService.EnrollInCourseAsync(studentId, courseId, ct);

            if (!result.IsSuccess)
            {
                return HandleErrorResult(result);
            }

            return Ok(new { Message = "Enrolled successfully" });
        }

        [HttpGet("my-courses")]
        public async Task<IActionResult> GetMyCourses(CancellationToken ct)
        {
            var studentId = GetCurrentStudentId();
            if (studentId == Guid.Empty) return Unauthorized();

            var result = await _studentService.GetMyCoursesAsync(studentId, ct);

            return Ok(result.Data);
        }
    }
}