using ResultPatternExample.Exceptions;

namespace ResultPatternExample.Responses;

public class PagingResponse<T> : Response<List<T>> where T : class
{
    public PagingResponse()
    {

    }

    public PagingResponse(PagingResponse<T> response)
    {
        PageNumber = response.PageNumber;
        PageSize = response.PageSize;
        RowCount = response.RowCount;
        PageCount = response.PageCount;
        Result = response.Result;
    }

    public PagingResponse(List<Error> errors, List<T> result, int statusCode) : base(errors, result, statusCode)
    {

    }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int RowCount { get; set; }
    public int PageCount { get; set; }

    public static PagingResponse<T> Success(PagingResponse<T> response)
    {
        return new(response);
    }
}
