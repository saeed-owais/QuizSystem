using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.API.Responses;
using System.Net;
using System.Text.Json;

namespace QuizSystem.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next,
                                           ILogger<ExceptionHandlingMiddleware> logger,
                                           IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred: {message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode;
            object responsePayload;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            context.Response.ContentType = "application/problem+json";

            switch (exception)
            {
                case ValidationException ex:
                    statusCode = (int)HttpStatusCode.BadRequest; // 400
                    responsePayload = new ValidationErrorResponse
                    {
                        Title = "Validation Error",
                        Status = statusCode,
                        Detail = "One or more validation errors occurred.",
                        Errors = ex.Errors
                                   .GroupBy(e => e.PropertyName, StringComparer.OrdinalIgnoreCase)
                                   .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray(), StringComparer.OrdinalIgnoreCase)
                    };
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    var (title, detail) = _env.IsDevelopment()
                        ? (exception.Message, exception.StackTrace?.ToString())
                        : ("An internal server error has occurred.", null);

                    responsePayload = new ProblemDetails
                    {
                        Title = title,
                        Status = statusCode,
                        Detail = detail
                    };
                    break;
            }

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(responsePayload, options));
        }
    }
}