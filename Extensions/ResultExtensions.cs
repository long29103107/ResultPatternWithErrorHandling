using ResultPatternExample.Exceptions;
using ResultPatternExample.Responses;

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
}