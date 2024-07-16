namespace ResultPatternExample.Repositories;

public interface ITodoRepository
{
    Task<List<Todo>> GetAllAsync();
}
