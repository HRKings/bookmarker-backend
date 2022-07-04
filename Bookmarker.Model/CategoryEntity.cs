namespace Bookmarker.Model;

public class CategoryEntity
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? ParentId { get; set; }
    public string? IconifyIcon { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}