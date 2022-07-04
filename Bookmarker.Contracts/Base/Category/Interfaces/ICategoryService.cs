using Bookmarker.Contracts.Utils;

namespace Bookmarker.Contracts.Base.Category.Interfaces;

public interface ICategoryService
{
    public Task<string?> Create(string title, string icon);
    public Task<Paginated<CategoryWithId>?> GetPaginated(int page, int pageSize);
    public Task<Paginated<CategoryWithId>?> GetTopLevelPaginated(int page, int pageSize);

}