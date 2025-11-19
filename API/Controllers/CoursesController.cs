using BLL.Dtos.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.BLL.Interfaces;
using QuizSystem.Common.Common;

namespace QuizSystem.API.Controllers
{
    [Authorize]
    public class CoursesController : ApiClientBaseController
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(Guid id, CancellationToken ct)
        {
            var result = await _courseService.GetCourseByIdAsync(id, ct);

            if (!result.IsSuccess)
            {
                return HandleErrorResult(result);
            }

            return Ok(result.Data);
        }

        [HttpGet("MyCourses")]
        public async Task<IActionResult> GetMyCourses(CancellationToken ct)
        {
            var instructorId = GetCurrentInstructorId();
            if (instructorId == Guid.Empty) return Unauthorized();

            var result = await _courseService.GetCoursesByInstructorAsync(instructorId, ct);

            return Ok(result.Data);
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.Instructor)]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto, CancellationToken ct)
        {
            var instructorId = GetCurrentInstructorId();
            if (instructorId == Guid.Empty) return Unauthorized();

            var result = await _courseService.CreateCourseAsync(dto, instructorId, ct);

            if (!result.IsSuccess)
            {
                return HandleErrorResult(result);
            }

            return CreatedAtAction(nameof(GetCourseById), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = AppRoles.Instructor)]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDto dto, CancellationToken ct)
        {
            var instructorId = GetCurrentInstructorId();
            if (instructorId == Guid.Empty) return Unauthorized();

            var result = await _courseService.UpdateCourseAsync(id, dto, instructorId, ct);

            if (!result.IsSuccess)
            {
                return HandleErrorResult(result);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Instructor)]
        public async Task<IActionResult> DeleteCourse(Guid id, CancellationToken ct)
        {
            var instructorId = GetCurrentInstructorId();
            if (instructorId == Guid.Empty) return Unauthorized();

            var result = await _courseService.DeleteCourseAsync(id, instructorId, ct);

            if (!result.IsSuccess)
            {
                return HandleErrorResult(result);
            }

            return NoContent();
        }
    }
}