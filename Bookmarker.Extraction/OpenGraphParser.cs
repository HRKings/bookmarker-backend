using Bookmarker.Model.Entities;
using HtmlAgilityPack;

namespace Bookmarker.Extraction;

public static class OpenGraphParser
{
    public static async Task<Bookmark?> GetOpenGraph(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;
        
        var web = new HtmlWeb();
        var doc = await web.LoadFromWebAsync(url);

        if (doc is null)
            return null;

        _ = doc.DocumentNode.InnerHtml;
        
        var headTitle = doc.DocumentNode.Descendants("title").FirstOrDefault()?.InnerText;

        var og = doc.DocumentNode.Descendants("meta")
            .Where(metaTag => metaTag.Attributes.Contains("property") && metaTag.Attributes["property"].Value.StartsWith("og:"))
            .DistinctBy(metaTag => metaTag.Attributes["property"].Value)
            .ToDictionary(metaTag => metaTag.Attributes["property"].Value.Replace("og:", string.Empty),
                metaTag => metaTag.Attributes["content"].Value);
        
        if (og.Count == 0) 
            return new Bookmark {Url = url, Title = headTitle ?? url};

        var title = og.ContainsKey("site_name") ? og["site_name"] : null;
        if (title is not null && og.ContainsKey("title"))
            title = string.Join(" | ", title, og["title"]);
        title = og.ContainsKey("title") && title is null ? og["title"] : title;

        var image = og.ContainsKey("image") ? og["image"] : null;
        image = string.IsNullOrWhiteSpace(image) && og.ContainsKey("image:url") ? og["image:url"] : image;

        if (!string.IsNullOrWhiteSpace(image) && image.StartsWith('/'))
        {
            var uri = new Uri(url);
            image = $"https://{uri.Host}{image}";
        }

        var video = og.ContainsKey("video") ? og["video"] : null;
        video = string.IsNullOrWhiteSpace(video) && og.ContainsKey("video:url") ? og["video:url"] : video;
        
        
        var result = new Bookmark
        {
            Url = url,
            Title = string.IsNullOrWhiteSpace(title) ? headTitle : title,
            Description = og.ContainsKey("description") ? og["description"] : null,
            OgType = og.ContainsKey("type") ? og["type"] : null,
            ImageUrl = image,
            ImageAlt = og.ContainsKey("image:alt") ? og["image:alt"] : null,
            VideoUrl = video,
        };

        return result;
    }
}