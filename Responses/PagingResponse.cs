namespace ResultPatternExample.Responses;

public class PagingResponse<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int RowCount { get; set; }
    public int PageCount { get; set; }
    public List<T> Results { get; set; }
}
