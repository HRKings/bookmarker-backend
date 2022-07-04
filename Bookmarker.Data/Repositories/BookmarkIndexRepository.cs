using System.Data;
using Bookmarker.Contracts.Base.Bookmark.Index;
using Bookmarker.Contracts.Base.Bookmark.Interfaces;
using Dapper;

namespace Bookmarker.Data.Repositories;

public class BookmarkIndexRepository : IBookmarkIndexRepository
{
    private readonly IDbConnection _dbConnection;

    public BookmarkIndexRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public Task<IEnumerable<BookmarkToIndex>?> GetBookmarksToBeFirstIndexed()
        => _dbConnection.QueryAsync<BookmarkToIndex>(Queries.BookmarkIndex.GET_TO_BASIC_INDEX);

    public Task<IEnumerable<string>?> GetBookmarksIdToBeFirstIndexed()
        => _dbConnection.QueryAsync<string>(Queries.BookmarkIndex.GET_ID_TO_BASIC_INDEX);

    public Task<int> SetIndexStatus(string id, bool hasBasicIndex, bool hasArticleIndex, bool hasContentIndex)
        => _dbConnection.ExecuteAsync(Queries.BookmarkIndex.SET_INDEX_STATUS, new {id, hasBasicIndex, hasArticleIndex, hasContentIndex});
}