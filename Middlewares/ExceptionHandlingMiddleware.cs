using ResultPatternExample.Exceptions;
using ResultPatternExample.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.IO;
using ResultPatternExample.Extensions;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

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
        Stream originalBody = context.Response.Body;
        var memStream = new MemoryStream();
        context.Response.Body = memStream;

        try
        {
            await _next.Invoke(context);

            var response = await _OverrideResponseAsync(memStream, context, originalBody);

            await _HandleResponseAsync(context, errorMsg, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            errorMsg = ex.Message;
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        }
        finally
        {
            if (!context.Response.HasStarted)
            {
                var successList = new List<int>()
                {
                    StatusCodes.Status204NoContent,
                    StatusCodes.Status202Accepted,
                    StatusCodes.Status200OK
                };

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
                else
                {
                    var response = await _OverrideResponseAsync(memStream, context, originalBody);

                    await _HandleResponseAsync(context, errorMsg, response);
                }
            }
        }
    }

    private async Task<Response> _OverrideResponseAsync(MemoryStream memStream, HttpContext context, Stream originalBody)
    {
        memStream.Position = 0;
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        string responseBody = await new StreamReader(memStream).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        var response = string.IsNullOrEmpty(responseBody)
            ? Response.Failure(new Error { Message = "Something went wrong !" }, context.Response.StatusCode)
            : JsonConvert.DeserializeObject<Response>(responseBody);

        context.Response.StatusCode = response.StatusCode;

        await memStream.CopyToAsync(originalBody);
        context.Response.Body = originalBody;

        return response;
    }

    private async Task _HandleResponseAsync(HttpContext context, string message, Response response = null)
    {
        context.Response.ContentType = "application/json";

        await WriteResultResponseAsync(context, response, message);
    }

    private async Task WriteResultResponseAsync(HttpContext context, Response response, string message)
    {
        var camelCaseFormatter = new JsonSerializerSettings();
        camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();

        var json = JsonConvert.SerializeObject(response ?? Response.Failure(new Error { Message = message }, context.Response.StatusCode), camelCaseFormatter);

        await context.Response.WriteAsync(json);
    }
}