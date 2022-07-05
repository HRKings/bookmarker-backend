using Bookmarker.Model;
using Bookmarker.Model.Entities;
using Bookmarker.Model.Utils;
using Microsoft.EntityFrameworkCore;

namespace Bookmarker.Data.Repositories;

public class BookmarkRepository
{
    private readonly BookmarkerContext _dbConnection;

    public BookmarkRepository(BookmarkerContext dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async ValueTask<Bookmark?> Create(Bookmark data)
    {
        var entity = await _dbConnection.Bookmarks.AddAsync(data);
        
        var success = await _dbConnection.SaveChangesAsync() == 1;
        if (!success)
            return null;
        
        return entity.Entity;
    }

    public ValueTask<Bookmark?> GetById(string id)
        => _dbConnection.Bookmarks.FindAsync(id.ToGuid());

    public Task<List<Bookmark>> GetPaginated(int limit, int offset)
        => _dbConnection.Bookmarks.Skip(offset).Take(limit).ToListAsync();

    public Task<int> GetTotalItems()
        => _dbConnection.Bookmarks.CountAsync();

    public Task<List<Bookmark>> GetActives()
        => _dbConnection.Bookmarks.Where(x => !(x.IsLinkBroken ?? false)).ToListAsync();
    
    public IAsyncEnumerable<Bookmark> GetDuplicatedLinks(string id, string url)
        => _dbConnection.Bookmarks.Where(x => x.PureUrl == url && x.Id.ToString() != id).AsAsyncEnumerable();

    public async Task<bool> FlagBrokenLink(string id)
    {
        var toUpdate = await _dbConnection.Bookmarks.FindAsync(id.ToGuid());

        if (toUpdate is null)
            return false;

        toUpdate.IsLinkBroken = true;

        _dbConnection.Bookmarks.Update(toUpdate);
        return await _dbConnection.SaveChangesAsync() == 1;
    }

    public async ValueTask<DuplicatedLink> FlagDuplicatedLink(string id, string duplicate)
    {
        var entity = await _dbConnection.DuplicatedLinks.AddAsync(new DuplicatedLink
        {
            OriginalId = id.ToGuid(),
            DuplicatedId = duplicate.ToGuid()
        });
        
        return entity.Entity;
    }

    public Task<List<Bookmark>> GetPaginatedByCategory(string categoryId, int limit, int offset)
        => _dbConnection.Bookmarks.Where(x => x.CategoryId.ToString() == categoryId).Skip(offset).Take(limit)
            .ToListAsync();

    public Task<int> GetItemsCountByCategory(string categoryId)
        => _dbConnection.Bookmarks.CountAsync(x => x.CategoryId.ToString() == categoryId);

    public async Task<Bookmark?> SetCategory(string id, string categoryId)
    {
        var toUpdate = await _dbConnection.Bookmarks.FindAsync(id.ToGuid());

        if (toUpdate is null)
            return null;

        toUpdate.CategoryId = categoryId.ToGuid();

        _dbConnection.Bookmarks.Update(toUpdate);
        var success = await _dbConnection.SaveChangesAsync() == 1;

        if (!success)
            return null;

        return toUpdate;
    }
}