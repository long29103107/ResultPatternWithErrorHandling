namespace ResultPatternExample.Services;

public interface IPostService
{
    Task<List<Todo>> GetAllAsync();
}
