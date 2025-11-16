using BLL.Dtos.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.BLL.Interfaces;
using QuizSystem.Common.Common;
using System.Security.Claims;

namespace QuizSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        private Guid GetCurrentInstructorId()
        {
            var instructorIdClaim = User.FindFirstValue(CustomClaimTypes.InstructorId);
            return Guid.TryParse(instructorIdClaim, out var id) ? id : Guid.Empty;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _courseService.GetCourseByIdAsync(id, cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Not Found",
                    Detail = result.Error,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(result.Data);
        }

        [HttpGet("MyCourses")]
        public async Task<IActionResult> GetMyCourses(CancellationToken cancellationToken)
        {
            var instructorId = GetCurrentInstructorId();
            if (instructorId == Guid.Empty)
            {
                return Unauthorized(new ProblemDetails { Title = "Unauthorized", Detail = "Invalid instructor token." });
            }

            var result = await _courseService.GetCoursesByInstructorAsync(instructorId, cancellationToken);

            return Ok(result.Data);
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.Instructor)]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto createCourseDto, CancellationToken cancellationToken)
        {
            var instructorId = GetCurrentInstructorId();
            if (instructorId == Guid.Empty)
            {
                return Unauthorized(new ProblemDetails { Title = "Unauthorized", Detail = "Invalid instructor token." });
            }

            var result = await _courseService.CreateCourseAsync(createCourseDto, instructorId, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Bad Request",
                    Detail = result.Error,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return CreatedAtAction(nameof(GetCourseById), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = AppRoles.Instructor)]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDto updateDto, CancellationToken cancellationToken)
        {
            var instructorId = GetCurrentInstructorId();
            if (instructorId == Guid.Empty)
            {
                return Unauthorized(new ProblemDetails { Title = "Unauthorized" });
            }

            var result = await _courseService.UpdateCourseAsync(id, updateDto, instructorId, cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Update Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Instructor)]
        public async Task<IActionResult> DeleteCourse(Guid id, CancellationToken cancellationToken)
        {
            var instructorId = GetCurrentInstructorId();
            if (instructorId == Guid.Empty)
            {
                return Unauthorized(new ProblemDetails { Title = "Unauthorized" });
            }

            var result = await _courseService.DeleteCourseAsync(id, instructorId, cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Delete Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return NoContent();
        }
    }
}