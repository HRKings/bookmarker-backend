using System.Data;
using Bookmarker.Contracts.Base.Category;
using Bookmarker.Contracts.Base.Category.Interfaces;
using Bookmarker.Model;
using Dapper;

namespace Bookmarker.Data.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly IDbConnection _dbConnection;

    public CategoryRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<string?> Create(string title, string icon)
    {
        var resultingId = await _dbConnection.QuerySingleAsync<string>(Queries.Category.INSERT, new {title, icon});

        return resultingId;
    }

    public Task<Category?> GetById(string id)
        => _dbConnection.QuerySingleOrDefaultAsync<Category?>(Queries.Category.GET_BY_ID, new {id});
    
    public Task<IEnumerable<CategoryEntity>?> GetPaginated(int limit, int offset)
        => _dbConnection.QueryAsync<CategoryEntity>(Queries.Category.GET_ALL_PAGINATED, new {limit, offset});

    public Task<int> GetTotalItems()
        => _dbConnection.QuerySingleAsync<int>(Queries.Category.GET_TOTAL_COUNT);

    public Task<IEnumerable<CategoryEntity>?> GetTopLevelPaginated(int limit, int offset)
        => _dbConnection.QueryAsync<CategoryEntity>(Queries.Category.GET_TOPLEVEL_PAGINATED, new {limit, offset});
    
    public Task<int> GetTopLevelItems()
        => _dbConnection.QuerySingleAsync<int>(Queries.Category.GET_TOPLEVEL_COUNT);
}