using Bookmarker.Model;

namespace Bookmarker.Contracts.Base.Bookmark.Search;

public class BookmarkSearchable: Bookmark
{
    public string? Id { get; set; }
    public string? CategoryName { get; set; }
    public string? ArticleContent { get; set; }
    public string? PageContent { get; set; }

    public static BookmarkSearchable FromBookmark(Bookmark from)
        => new()
        {
            Url = from.Url,
            Title = from.Title,
            OgType = from.OgType,
            ImageUrl = from.ImageUrl,
            ImageAlt = from.ImageAlt,
            VideoUrl = from.VideoUrl,
            Description = from.Description,
            CategoryName = "Unsorted",
        };
    
    public static BookmarkSearchable FromBookmarkEntity(BookmarkEntity from)
        => new()
        {
            Id = from.Id,
            Url = from.Url,
            Title = from.Title,
            OgType = from.OgType,
            ImageUrl = from.ImageUrl,
            ImageAlt = from.ImageAlt,
            VideoUrl = from.VideoUrl,
            Description = from.Description,
            CategoryName = "Unsorted",
        };

    public static BookmarkSearchable FromBookmarkWithId(string id, Bookmark from)
    {
        var result = FromBookmark(from);
        result.Id = id;
        return result;
    }
}