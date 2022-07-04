using Bookmarker.Contracts.Utils;

namespace Bookmarker.Contracts.Base.Bookmark.Interfaces;

public interface IBookmarkService
{
    /// <summary>
    /// Create a new bookmark with the request data as it is
    /// </summary>
    /// <param name="data">The data sent with the request</param>
    /// <returns>The ID of the bookmark or null in case of the creation fails</returns>
    public Task<string?> CreateFull(Bookmark data);
    
    /// <summary>
    /// Creates a new bookmark from the parsed data, the null fields will be completed by the request data
    /// </summary>
    /// <param name="data">The data sent with the request</param>
    /// <returns>A tuple with the first field being the bookmark ID and the second one being the created bookmark
    /// or null in case the creation fails</returns>
    public Task<(string, Bookmark)?> CreateWithRequestFallback(Bookmark data);
    
    /// <summary>
    /// Creates a new bookmark from the request data, the null fields will be completed by the parsed data
    /// </summary>
    /// <param name="data">The data sent with the request</param>
    /// <returns>A tuple with the first field being the bookmark ID and the second one being the created bookmark
    /// or null in case the creation fails</returns>
    public Task<(string, Bookmark)?> CreateWithParsedFallback(Bookmark data);

    public Task<Paginated<Bookmark>?> Search(string query,  string? category, int page, int pageSize);
    
    public Task<Paginated<BookmarkCategorized>?> GetPaginated(int page, int pageSize);

    public Task<Paginated<BookmarkCategorized>?> GetPaginatedByCategory(string categoryId, int page, int pageSize);

    public Task<bool> SetCategory(string id, string categoryId);
}