using Bookmarker.Contracts.Utils;
using Bookmarker.Data.Repositories;
using Bookmarker.Model.Entities;

namespace Bookmarker.API.Services;

public class CategoryService
{
    private readonly CategoryRepository _repository;

    public CategoryService(CategoryRepository repository)
    {
        _repository = repository;
    }

    public ValueTask<Category?> Create(string title, string icon)
        => _repository.Create(title, icon);

    public async Task<Paginated<Category>?> GetPaginated(int page, int pageSize)
    {
        var itemCount = await _repository.GetTotalItems();

        if (itemCount == 0)
            return null;

        var entities = await _repository.GetPaginated(pageSize, (page - 1) * pageSize);

        if (entities.Count == 0)
            return null;

        var totalPages = (int) Math.Ceiling(itemCount / (double) pageSize);
        return new Paginated<Category>
        {
            Content = entities,
            Page = page,
            ItemsPerPage = pageSize,
            TotalPages = totalPages,
            TotalItems = itemCount,
            HasNext = page < totalPages,
        };
    }

    public async Task<Paginated<Category>?> GetTopLevelPaginated(int page, int pageSize)
    {
        var itemCount = await _repository.GetTopLevelItems();

        if (itemCount == 0)
            return null;

        var entities = await _repository.GetTopLevelPaginated(pageSize, (page - 1) * pageSize);

        if (entities.Count == 0)
            return null;

        var totalPages = (int) Math.Ceiling(itemCount / (double) pageSize);
        return new Paginated<Category>
        {
            Content = entities,
            Page = page,
            ItemsPerPage = pageSize,
            TotalPages = totalPages,
            TotalItems = itemCount,
            HasNext = page < totalPages,
        };
    }

    public async Task<bool> Delete(string id)
    {
        var rows = await _repository.Delete(id);
        return rows == 1;
    }
}