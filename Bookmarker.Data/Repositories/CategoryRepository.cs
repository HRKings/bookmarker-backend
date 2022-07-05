using Bookmarker.Model;
using Bookmarker.Model.Entities;
using Bookmarker.Model.Utils;
using Microsoft.EntityFrameworkCore;

namespace Bookmarker.Data.Repositories;

public class CategoryRepository
{
    private readonly BookmarkerContext _dbConnection;

    public CategoryRepository(BookmarkerContext dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async ValueTask<Category?> Create(string title, string icon)
    {
        var entity = await _dbConnection.Categories.AddAsync(new Category
        {
            Title = title,
            Icon = icon,
        });
        
        var success = await _dbConnection.SaveChangesAsync() == 1;
        if (!success)
            return null;
        
        return entity.Entity;
    }

    public ValueTask<Category?> GetById(string id)
        => _dbConnection.Categories.FindAsync(id.ToGuid());

    public Task<List<Category>> GetPaginated(int limit, int offset)
        => _dbConnection.Categories.Skip(offset).Take(limit).ToListAsync();

    public Task<int> GetTotalItems()
        => _dbConnection.Bookmarks.CountAsync();

    public Task<List<Category>> GetTopLevelPaginated(int limit, int offset)
        => _dbConnection.Categories.Where(x => x.ParentId == null).Skip(offset).Take(limit).ToListAsync();
    
    public Task<int> GetTopLevelItems()
        => _dbConnection.Bookmarks.CountAsync();

    public Task<int> Delete(string id)
    {
        _dbConnection.Categories.Remove(new Category
        {
            Id = id.ToGuid(),
        });

        return _dbConnection.SaveChangesAsync();
    }
}