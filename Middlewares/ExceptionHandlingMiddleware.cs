using ResultPatternExample.Exceptions;
using ResultPatternExample.Responses;
using System.Text.Json;

namespace ResultPatternExample.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occurred: {Message}", e.Message);

            context.Response.ContentType = "application/json";

            await _HandleExceptionAsync(context, StatusCodes.Status503ServiceUnavailable, e.Message);
        }
    }

    private async Task _HandleExceptionAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

        var response =  Response.Failure(new Error { Message = message }, statusCode);
      
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}