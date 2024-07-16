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
        // Buffer the response body
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        try
        {
            await _next(context);

            // Log the response
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation($"Response: {context.Response.StatusCode}, Body: {responseText}");

            // Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occurred: {Message}", e.Message);

            context.Response.ContentType = "application/json";

            await _HandleExceptionAsync(context, StatusCodes.Status503ServiceUnavailable, e.Message);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task _HandleExceptionAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

        var response =  Response.Failure(new Error { Message = message }, statusCode);
      
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}