namespace Bookmarker.Contracts.Base.Bookmark;

public class Bookmark
{
    public string? Id { get; set; }
    public string? Url { get; set; }
    public string? PureUrl
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Url))
                return null;
            
            var uri = new Uri(Url);
            return $"{uri.Host}{uri.LocalPath}".Trim('/', '\\').Replace("www.", "");
        }
    }

    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageAlt { get; set; }
    public string? OgType { get; set; }
    public string? VideoUrl { get; set; }
    public bool IsLinkBroken { get; set; }
}