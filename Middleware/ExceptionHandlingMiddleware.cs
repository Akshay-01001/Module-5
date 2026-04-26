using System.Net;
using UserManagementApi.Exceptions;

namespace UserManagementApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            if (ex is BadRequestException)
            {
                _logger.LogWarning(ex, "Bad request.");

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = ex.Message
                });

                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            _logger.LogError(ex, "Unhandled exception occurred while processing the request.");

            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "An unexpected error occurred. Please try again later."
            });
        }
    }
}
