namespace Bookmarker.Model;

public class BookmarkEntity
{
    public string Id { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageAlt { get; set; }
    public string? OgType { get; set; }
    public string? VideoUrl { get; set; }
    public string? CategoryId { get; set; }
    public bool IsLinkBroken { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}