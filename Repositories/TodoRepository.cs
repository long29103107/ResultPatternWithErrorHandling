namespace ResultPatternExample.Repositories;

public class TodoRepository : ITodoRepository
{
    private static HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://jsonplaceholder.typicode.com"),
    };

    public async Task<List<Todo>> GetAllAsync()
    {
        var result = (await _httpClient.GetFromJsonAsync<List<Todo>>("todos"))
            .ToList()
            .Take(5)
            .ToList();

        return result ?? new List<Todo>();
    }
}
