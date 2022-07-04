using Bookmarker.Model;

namespace Bookmarker.Contracts.Base.Bookmark.Interfaces;

public interface IBookmarkRepository
{
    Task<string?> Create(Bookmark content);

    Task<Bookmark?> GetById(string id);
    Task<IEnumerable<BookmarkEntity>?> GetPaginated(int limit, int offset);
    Task<int> GetTotalItems();
    
    Task<IEnumerable<(string, string)>?> GetActiveIdsAndUrls();
    Task<IEnumerable<(string, string)>?> GetActiveIdsAndPureUrls();
    Task<IEnumerable<string>?> GetDuplicatedLinks(string id, string url);

    Task<int> FlagBrokenLink(string id);
    Task<int> FlagDuplicatedLink(string id, string duplicate);
    
    Task<IEnumerable<BookmarkEntity>?> GetPaginatedByCategory(string categoryId, int limit, int offset);
    Task<int> GetItemsCountByCategory(string categoryId);

    Task<BookmarkEntity?> SetCategory(string id, string categoryId);
}