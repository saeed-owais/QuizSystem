using Microsoft.AspNetCore.Mvc;
using QuizSystem.Common.Common;
using System.Security.Claims;

namespace QuizSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiClientBaseController : ControllerBase
    {
        protected IActionResult HandleErrorResult(Result result)
        {
            var problemDetails = new ProblemDetails
            {
                Title = GetTitleForErrorType(result.ErrorType),
                Detail = result.Error,
                Status = GetStatusCodeForErrorType(result.ErrorType)
            };

            return StatusCode(problemDetails.Status.Value, problemDetails);
        }


        protected Guid GetCurrentInstructorId()
        {
            var claim = User.FindFirstValue(CustomClaimTypes.InstructorId);
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }


        private static int GetStatusCodeForErrorType(ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Failure => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        private static string GetTitleForErrorType(ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.NotFound => "Resource Not Found",
                ErrorType.Validation => "Validation Error",
                ErrorType.Conflict => "Conflict",
                ErrorType.Unauthorized => "Forbidden Operation",
                ErrorType.Failure => "Bad Request",
                _ => "An error occurred"
            };
        }
    }
}