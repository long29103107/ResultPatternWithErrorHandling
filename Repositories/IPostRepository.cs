namespace ResultPatternExample.Repositories;

public interface IPostRepository
{
    Task<List<Todo>> GetAllAsync();
}
