using Bookmarker.Contracts.Base.Bookmark;
using Bookmarker.Contracts.Base.Category;
using Bookmarker.Contracts.Base.Category.Interfaces;
using Bookmarker.Contracts.Utils;

namespace Bookmarker.API.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<string?> Create(string title, string icon)
    {
        var result = await _repository.Create(title, icon);

        return result ?? null;
    }

    public async Task<Paginated<CategoryWithId>?> GetPaginated(int page, int pageSize)
    {
        var itemCount = await _repository.GetTotalItems();

        if (itemCount == 0)
            return null;

        var entities = await _repository.GetPaginated(pageSize, (page - 1) * pageSize);

        if (entities is null)
            return null;

        var totalPages = (int) Math.Ceiling(itemCount / (double) pageSize);
        return new Paginated<CategoryWithId>
        {
            Content = entities.Select(entity => new CategoryWithId
            {
                Id = entity.Id,
                Title = entity.Title,
                ParentId = entity.ParentId,
                IconifyIcon = entity.IconifyIcon
            }),
            Page = page,
            ItemsPerPage = pageSize,
            TotalPages = totalPages,
            TotalItems = itemCount,
            HasNext = page < totalPages,
        };
    }

    public async Task<Paginated<CategoryWithId>?> GetTopLevelPaginated(int page, int pageSize)
    {
        var itemCount = await _repository.GetTopLevelItems();

        if (itemCount == 0)
            return null;

        var entities = await _repository.GetTopLevelPaginated(pageSize, (page - 1) * pageSize);

        if (entities is null)
            return null;

        var totalPages = (int) Math.Ceiling(itemCount / (double) pageSize);
        return new Paginated<CategoryWithId>
        {
            Content = entities.Select(entity => new CategoryWithId
            {
                Id = entity.Id,
                Title = entity.Title,
                ParentId = entity.ParentId,
                IconifyIcon = entity.IconifyIcon
            }),
            Page = page,
            ItemsPerPage = pageSize,
            TotalPages = totalPages,
            TotalItems = itemCount,
            HasNext = page < totalPages,
        };
    }
}