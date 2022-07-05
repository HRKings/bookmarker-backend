using Bookmarker.Model;
using Bookmarker.Model.Entities;
using Bookmarker.Model.Utils;
using Microsoft.EntityFrameworkCore;

namespace Bookmarker.Data.Repositories;

public class BookmarkIndexRepository
{
    private readonly BookmarkerContext _dbConnection;

    public BookmarkIndexRepository(BookmarkerContext dbConnection)
    {
        _dbConnection = dbConnection;
    }
    
    public async ValueTask<BookmarkIndexStatus?> Create(BookmarkIndexStatus data)
    {
        var entity = await _dbConnection.BookmarkIndexStatuses.AddAsync(data);
        
        return entity.Entity;
    }

    public Task<List<Bookmark>> GetBookmarksToBeFirstIndexed()
    => _dbConnection.BookmarkIndexStatuses.Where(x => !x.HasBasicIndex)
        .Include(x => x.IdNavigation)
        .Include(x => x.IdNavigation.Category)
        .Select(x => x.IdNavigation).ToListAsync();

    public Task<List<string>> GetBookmarksIdToBeFirstIndexed()
        => _dbConnection.BookmarkIndexStatuses.Where(x => !x.HasBasicIndex)
            .Select(x => x.Id.ToString()).ToListAsync();

    public async Task<bool> SetIndexStatus(string id, bool hasBasicIndex, bool hasArticleIndex, bool hasContentIndex)
    {
        var toUpdate = await _dbConnection.BookmarkIndexStatuses.FindAsync(id.ToGuid());

        if (toUpdate is null)
            return false;

        toUpdate.HasBasicIndex = hasBasicIndex;
        toUpdate.HasArticleIndex = hasArticleIndex;
        toUpdate.HasContentIndex = hasContentIndex;

        _dbConnection.BookmarkIndexStatuses.Update(toUpdate);
        return await _dbConnection.SaveChangesAsync() == 1;
    }
}