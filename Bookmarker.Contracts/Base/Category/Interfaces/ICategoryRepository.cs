using Bookmarker.Model;

namespace Bookmarker.Contracts.Base.Category.Interfaces;

public interface ICategoryRepository
{
    Task<string?> Create(string title, string icon);
    Task<Category?> GetById(string id);
    Task<IEnumerable<CategoryEntity>?> GetPaginated(int limit, int offset);
    Task<int> GetTotalItems();
    
    Task<IEnumerable<CategoryEntity>?> GetTopLevelPaginated(int limit, int offset);
    Task<int> GetTopLevelItems();
}