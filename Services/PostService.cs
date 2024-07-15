using ResultPatternExample.Repositories;

namespace ResultPatternExample.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _repository;

    public PostService(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Todo>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }
}