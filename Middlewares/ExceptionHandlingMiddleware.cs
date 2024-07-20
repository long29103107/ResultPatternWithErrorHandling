using ResultPatternExample.Exceptions;
using ResultPatternExample.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
    public async Task Invoke(HttpContext context)
    {
        var errorMsg = string.Empty;
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            errorMsg = ex.Message;
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        }

        var successList = new List<int>()
        {
            StatusCodes.Status204NoContent,
            StatusCodes.Status202Accepted,
            StatusCodes.Status200OK
        };

        if (!context.Response.HasStarted)
        {
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                await _HandleResponseAsync(context, "Unauthorized");
            }
            else if (!context.Response.HasStarted && context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                await _HandleResponseAsync(context, "Forbidden");
            }
            else if (successList.TrueForAll(x => x != context.Response.StatusCode))
            {
                await _HandleResponseAsync(context, errorMsg);
            }
        }
    }

    private async Task _HandleResponseAsync(HttpContext context, string message)
    {
        context.Response.ContentType = "application/json";

        var response = Response.Failure(new Error { Message = message }, context.Response.StatusCode);

        var camelCaseFormatter = new JsonSerializerSettings();
        camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();

        var json = JsonConvert.SerializeObject(response, camelCaseFormatter);
        await context.Response.WriteAsync(json);
    }
}