namespace Bookmarker.Contracts.Base;

public class SearchBookmark
{
    public string? Id { get; set; }
    public string Url { get; set; } = null!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageAlt { get; set; }
    public string? OgType { get; set; }
    public string? VideoUrl { get; set; }
    public string? CategoryName { get; set; }
    public string? PureUrl { get; set; }
    
    public string? ArticleContent { get; set; }
    public string? PageContent { get; set; }

    public static SearchBookmark FromEntity(Model.Entities.Bookmark entity)
        => new()
        {
            Id = entity.Id.ToString(),
            Description = entity.Description,
            Title = entity.Title,
            Url = entity.Url,
            CategoryName = entity.Category?.Title ?? "Unsorted",
            ImageAlt = entity.ImageAlt,
            ImageUrl = entity.ImageUrl,
            OgType = entity.OgType,
            PureUrl = entity.PureUrl,
            VideoUrl = entity.VideoUrl,
        };
}