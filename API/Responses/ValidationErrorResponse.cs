using Microsoft.AspNetCore.Mvc;

namespace QuizSystem.API.Responses
{
    public class ValidationErrorResponse : ProblemDetails
    {
        public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
    }
}