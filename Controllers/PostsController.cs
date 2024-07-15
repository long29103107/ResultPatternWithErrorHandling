using Microsoft.AspNetCore.Mvc;
using ResultPatternExample.Services;

namespace ResultPatternExample.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IPostService _service;

    public PostsController(IPostService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        return Ok(await _service.GetAllAsync());
    }
}
