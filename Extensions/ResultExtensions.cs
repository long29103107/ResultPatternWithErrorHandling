using ResultPatternExample.Exceptions;
using ResultPatternExample.Responses;
using System.Text;

namespace ResultPatternExample.Extensions;

public static class ResultExtensions
{
    public static T Match<T>(
        this Response<T> result,
        Func<T> onSuccess,
        Func<List<Error>, T> onFailure)
        where T : class
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Errors);
    }

    public static async Task<string> ReadAsString(this HttpResponse response)
    {
        var initialBody = response.Body;
        var buffer = new byte[Convert.ToInt32(response.ContentLength)];
        await response.Body.ReadAsync(buffer, 0, buffer.Length);
        var body = Encoding.UTF8.GetString(buffer);
        response.Body = initialBody;
        return body;
    }
}