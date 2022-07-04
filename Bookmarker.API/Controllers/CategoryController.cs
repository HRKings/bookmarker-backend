using Bookmarker.Contracts.Base.Category.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bookmarker.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ILogger<CategoryController> _logger;
    private readonly ICategoryService _service;

    public CategoryController(ILogger<CategoryController> logger, ICategoryService service)
    {
        _logger = logger;
        _service = service;
    }

    public record struct CreateCategoryContract(string Title, string? Icon);
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryContract request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest();
        
        var result = await _service.Create(request.Title, request.Icon ?? "bi:bookmark-star");

        if (result is null)
            return StatusCode(500);
        
        return Created($"api/v1/{result}", request);
    }
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAllPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var results = await _service.GetPaginated(page, pageSize);
        
        if (results is null)
            return NoContent();

        return Ok(results);
    }
    
    [HttpGet("toplevel")]
    public async Task<IActionResult> GetTopLevelPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var results = await _service.GetTopLevelPaginated(page, pageSize);
        
        if (results is null)
            return NoContent();

        return Ok(results);
    }
}