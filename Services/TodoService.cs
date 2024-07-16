using AutoMapper;
using FluentValidation;
using ResultPatternExample.Exceptions;
using ResultPatternExample.Repositories;
using ResultPatternExample.Requests;
using ResultPatternExample.Responses;

namespace ResultPatternExample.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;
    private readonly IValidator<Todo> _validator;
    private readonly IMapper _mapper;

    public TodoService(ITodoRepository repository, IValidator<Todo> validator, IMapper mapper)
    {
        _repository = repository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Response<List<Todo>>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync();

        return Response<List<Todo>>.Success(result);
    }

    public async Task<Response<Todo>> GetAsync(int id)
    {
        var result = (await _repository.GetAllAsync()).FirstOrDefault(x => x.Id == id);

        if(result == null)
        {
            return Response<Todo>.Failure(new Error
            {
                Message = $"Post {id} is not found !"
            }, 
            StatusCodes.Status404NotFound);
        }

        return Response<Todo>.Success(result);
    }

    public async Task<Response<Todo>> CreateAsync(CreateTodoRequest request)
    {
        var todo = new Todo()
        {
            UserId = request.UserId,
            Title = request.Title,
            Completed = request.Completed
        };

        var result = await _validator.ValidateAsync(todo);

        if (!result.IsValid)
        {
            var error = _mapper.Map<List<Error>>(result.Errors);
            return Response<Todo>.Failure(error, StatusCodes.Status400BadRequest);
        }

        return Response<Todo>.Success(todo);
    }
}