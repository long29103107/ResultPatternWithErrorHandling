using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResultPatternExample.Requests;
using ResultPatternExample.Responses;
using ResultPatternExample.Services;

namespace ResultPatternExample.Controllers;
[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class TodosController : ControllerBase
{
    private readonly ITodoService _service;

    public TodosController(ITodoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id)
    {
        return Ok(await _service.GetAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTodoRequest request)
    {
        return Ok(await _service.CreateAsync(request));
    }
}
