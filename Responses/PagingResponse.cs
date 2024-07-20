using ResultPatternExample.Exceptions;

namespace ResultPatternExample.Responses;

public class PagingResponse<T> : Response<List<T>> where T : class
{
    public PagingResponse()
    {
    }

    public PagingResponse(List<Error> errors, List<T> result, int statusCode) : base(errors, result, statusCode)
    {
        if (!errors.Any())
        {
            Result = result ?? (List<T>)Activator.CreateInstance(typeof(List<T>));
        }
    }

    public PagingResponse(PagingResponse<T> response)
    {
        PageNumber = response.PageNumber;
        PageSize = response.PageSize;
        RowCount = response.RowCount;
        PageCount = response.PageCount;
        Result = response.Result;
    }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int RowCount { get; set; }
    public int PageCount { get; set; }

    public static PagingResponse<T> Success(PagingResponse<T> response)
    {
        return new(response);
    }

    public static PagingResponse<T> Failure(List<Error> errors, int statusCode)
    {
        return new(errors, default(List<T>), statusCode);
    }

    public static PagingResponse<T> Failure(Error error, int statusCode)
    {
        return new(new List<Error>() { error }, default(List<T>), statusCode);
    }
}
