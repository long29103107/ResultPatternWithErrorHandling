namespace ResultPatternExample.Repositories;

public class PostRepository : IPostRepository
{
    private static HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://jsonplaceholder.typicode.com"),
    };

    public async Task<List<Todo>> GetAllAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<Todo>>("todos");

        return result ?? new List<Todo>();
    }
}
