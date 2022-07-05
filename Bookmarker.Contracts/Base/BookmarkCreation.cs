namespace Bookmarker.Contracts.Base;

public class BookmarkCreation
{
    public string Url { get; set; } = null!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageAlt { get; set; }
    public string? OgType { get; set; }
    public string? VideoUrl { get; set; }
    public string? CategoryName { get; set; }
    public string? PureUrl { get; set; }

    public Model.Entities.Bookmark ToEntity()
        => new()
        {
            Description = Description,
            Title = Title,
            Url = Url,
            ImageAlt = ImageAlt,
            ImageUrl = ImageUrl,
            OgType = OgType,
            PureUrl = PureUrl,
            VideoUrl = VideoUrl,
        };
}