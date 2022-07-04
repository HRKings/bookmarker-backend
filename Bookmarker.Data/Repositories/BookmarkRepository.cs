using System.Data;
using Bookmarker.Contracts;
using Bookmarker.Contracts.Base;
using Bookmarker.Contracts.Base.Bookmark;
using Bookmarker.Contracts.Base.Bookmark.Interfaces;
using Bookmarker.Model;
using Dapper;

namespace Bookmarker.Data.Repositories;

public class BookmarkRepository : IBookmarkRepository
{
    private readonly IDbConnection _dbConnection;

    public BookmarkRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<string?> Create(Bookmark content)
    {
        var resultingId = await _dbConnection.QuerySingleAsync<string>(Queries.Bookmark.INSERT, new
        {
            url = content.Url, 
            pureUrl = content.PureUrl,
            title = content.Title, 
            desc = content.Description, 
            image = content.ImageUrl,
            alt = content.ImageAlt, 
            type = content.OgType, 
            video = content.VideoUrl
        });

        return resultingId;
    }

    public Task<Bookmark?> GetById(string id)
        => _dbConnection.QuerySingleOrDefaultAsync<Bookmark?>(Queries.Bookmark.GET_BY_ID, new {id});

    public Task<IEnumerable<BookmarkEntity>?> GetPaginated(int limit, int offset)
        => _dbConnection.QueryAsync<BookmarkEntity>(Queries.Bookmark.GET_ALL_PAGINATED, new {limit, offset});

    public Task<int> GetTotalItems()
        => _dbConnection.QuerySingleAsync<int>(Queries.Bookmark.GET_TOTAL_COUNT);

    public Task<IEnumerable<(string, string)>?> GetActiveIdsAndUrls()
        => _dbConnection.QueryAsync<(string, string)>(Queries.Bookmark.GET_ACTIVE_IDS_URLS);

    public Task<IEnumerable<(string, string)>?> GetActiveIdsAndPureUrls()
        => _dbConnection.QueryAsync<(string, string)>(Queries.Bookmark.GET_ACTIVE_IDS_PURE_URLS);

    public Task<IEnumerable<string>?> GetDuplicatedLinks(string id, string url)
        => _dbConnection.QueryAsync<string>(Queries.Bookmark.GET_DUPLICATED_LINKS, new {id, url});

    public Task<int> FlagBrokenLink(string id)
        => _dbConnection.ExecuteAsync(Queries.Bookmark.FLAG_BROKEN_LINK, new {id});

    public Task<int> FlagDuplicatedLink(string id, string duplicate)
        => _dbConnection.ExecuteAsync(Queries.DuplicatedLinks.FLAG_DUPLICATED_LINK, new {id, duplicate});

    public Task<IEnumerable<BookmarkEntity>?> GetPaginatedByCategory(string categoryId, int limit, int offset)
        => _dbConnection.QueryAsync<BookmarkEntity>(Queries.Bookmark.GET_PAGINATED_BY_CATEGORY, new {categoryId, limit, offset});

    public Task<int> GetItemsCountByCategory(string categoryId)
        => _dbConnection.QuerySingleAsync<int>(Queries.Bookmark.GET_COUNT_BY_CATEGORY, new {categoryId});

    public Task<BookmarkEntity?> SetCategory(string id, string categoryId)
        => _dbConnection.QuerySingleAsync<BookmarkEntity?>(Queries.Bookmark.SET_CATEGORY, new {id, categoryId});
}