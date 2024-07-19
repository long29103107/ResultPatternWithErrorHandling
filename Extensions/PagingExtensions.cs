using ResultPatternExample.Responses;

namespace ResultPatternExample.Extensions;

public static class PagingExtensions
{
    public static PagingResponse<T> GetMakeList<T>(this List<T> queryset, PagingRequest request) 
        where T : class
    {
        var result = new PagingResponse<T>();

        result.PageNumber = request.PageNumber;
        result.PageSize = request.PageSize;
        result.RowCount = queryset.Count();

        var pageCount = (double)result.RowCount / result.PageSize;
        result.PageCount = (int)Math.Ceiling(pageCount);

        var skip = (result.PageNumber - 1) * result.PageSize;
        var take = result.PageSize;

        result.Result = queryset
            .Skip(skip)
            .Take(take)
            .ToList();

        return PagingResponse<T>.Success(result);
    }
}
