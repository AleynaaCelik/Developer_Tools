using DevTools.Core.Exceptions;
using System.Net;
using System.Text.Json;

namespace DevTools.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = new
                {
                    message = exception.Message,
                    type = exception.GetType().Name,
                    timestamp = DateTime.UtcNow
                }
            };

            switch (exception)
            {
                case UserLimitExceededException:
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    break;
                case InvalidAnalysisRequestException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case CodeAnalysisException:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = new
                    {
                        error = new
                        {
                            message = "An internal server error occurred",
                            type = "InternalServerError",
                            timestamp = DateTime.UtcNow
                        }
                    };
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}

