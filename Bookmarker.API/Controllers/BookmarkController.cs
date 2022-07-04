using Bookmarker.Contracts;
using Bookmarker.Contracts.Base;
using Bookmarker.Contracts.Base.Bookmark;
using Bookmarker.Contracts.Base.Bookmark.Interfaces;
using Bookmarker.Extraction;
using Microsoft.AspNetCore.Mvc;

namespace Bookmarker.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BookmarkController : ControllerBase
{
    private readonly ILogger<BookmarkController> _logger;
    private readonly IBookmarkService _service;

    public BookmarkController(ILogger<BookmarkController> logger, IBookmarkService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpPost("full")]
    public async Task<IActionResult> CreateFull([FromBody] Bookmark request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
            return BadRequest();
        
        var result = await _service.CreateFull(request);

        if (result is null)
            return StatusCode(500);
        
        return Created($"api/v1/{result}", request);
    }
    
    [HttpPost("url")]
    public async Task<IActionResult> CreateByUrl([FromBody] Bookmark request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
            return BadRequest();
        
        var result = await _service.CreateWithRequestFallback(request);

        if (result is null)
            return StatusCode(500);
        
        return Created($"api/v1/{result.Value.Item1}", result.Value.Item2);
    }
    
    [HttpPost("complete")]
    public async Task<IActionResult> CreateByRequest([FromBody] Bookmark request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
            return BadRequest();
        
        var result = await _service.CreateWithParsedFallback(request);

        if (result is null)
            return StatusCode(500);
        
        return Created($"api/v1/{result.Value.Item1}", result.Value.Item2);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var results = await _service.GetPaginated(page, pageSize);
        
        if (results is null)
            return NoContent();

        return Ok(results);
    }
    
    [HttpGet("{categoryId}")]
    public async Task<IActionResult> GetAllPaginated([FromRoute] string categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var results = await _service.GetPaginatedByCategory(categoryId, page, pageSize);
        
        if (results is null)
            return NoContent();

        return Ok(results);
    }
    
    [HttpGet("search/{query}")]
    public async Task<IActionResult> Search([FromRoute] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _service.Search(query, null, page, pageSize);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    public record struct SearchBody(string Query, string? Category, int? Page, int? PageSize);
    
    [HttpPost("search")]
    public async Task<IActionResult> SearchWithBody([FromBody] SearchBody request)
    {
        var result = await _service.Search(request.Query, request.Category, request.Page ?? 1, request.PageSize ?? 20);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    public record struct CategoryContainer(string Category);
    [HttpPut("{id}/category")]
    public async Task<IActionResult> SetCategory([FromRoute] string id, [FromBody] CategoryContainer body)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(body.Category))
            return BadRequest();
        
        var result = await _service.SetCategory(id, body.Category);

        if (!result)
            return BadRequest();

        return Ok();
    }
    
    public record struct UrlContainer(string Url);

    [HttpGet("og")]
    public async Task<IActionResult> SearchOg([FromBody] UrlContainer request)
    {
        var result = await OpenGraphParser.GetOpenGraph(request.Url);

        if (result is null)
            return NotFound();

        return new OkObjectResult(result);
    }
}