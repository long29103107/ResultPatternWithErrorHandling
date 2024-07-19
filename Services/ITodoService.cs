using ResultPatternExample.Requests;
using ResultPatternExample.Responses;

namespace ResultPatternExample.Services;

public interface ITodoService
{
    Task<Response<List<Todo>>> GetAllAsync();
    Task<Response<Todo>> GetAsync(int id);
    Task<PagingResponse<Todo>> GetMakeListAsync(PagingRequest request);
    Task<Response> CreateAsync(CreateTodoRequest request);
}
