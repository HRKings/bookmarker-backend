using Bookmarker.Contracts;
using Bookmarker.Contracts.Base;
using Bookmarker.Contracts.Base.Bookmark;
using Bookmarker.Contracts.Base.Bookmark.Interfaces;
using Bookmarker.Contracts.Base.Bookmark.Search;
using Bookmarker.Contracts.Base.Category.Interfaces;
using Bookmarker.Contracts.Utils;
using Bookmarker.Extraction;
using Bookmarker.Search;
using Meilisearch;
using Index = Meilisearch.Index;

namespace Bookmarker.API.Services;

public class BookmarkService : IBookmarkService
{
    private readonly IBookmarkRepository _repository;
    private readonly Index _index;
    
    private readonly ICategoryRepository _categoryRepository;


    public BookmarkService(IBookmarkRepository repository, IndexWrapper<BookmarkSearchable> indexWrapper, ICategoryRepository categoryRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _index = indexWrapper.Index;
    }

    public async Task<string?> CreateFull(Bookmark data)
    {
        var result = await _repository.Create(data);

        if (result is null)
            return null;

        var _ = await _index.AddDocumentsAsync(new []
        {
            BookmarkSearchable.FromBookmarkWithId(result, data)
        });
        
        return result;
    }

    public async Task<(string, Bookmark)?> CreateWithRequestFallback(Bookmark data)
    {
        if (data.Url is null)
            return null;
        
        var parsedData = await OpenGraphParser.GetOpenGraph(data.Url) ?? new Bookmark();

        foreach (var property in parsedData.GetType().GetProperties())
        {
            if(property.GetValue(parsedData) is null)
                property.SetValue(parsedData, property.GetValue(data));
        }
        
        var result  = await _repository.Create(parsedData);
        
        if (result is null)
            return null;
        
        var _ = await _index.AddDocumentsAsync(new []
        {
            BookmarkSearchable.FromBookmarkWithId(result, parsedData)
        });
        
        return (result, parsedData);
    }
    
    public async Task<(string, Bookmark)?> CreateWithParsedFallback(Bookmark data)
    {
        if (data.Url is null)
            return null;
        
        var parsedData = await OpenGraphParser.GetOpenGraph(data.Url) ?? new Bookmark();
        
        foreach (var property in data.GetType().GetProperties())
        {
            if(property.GetValue(data) is null)
                property.SetValue(data, property.GetValue(parsedData));
        }
        
        var result  = await _repository.Create(data);
        
        if (result is null)
            return null;
        
        var _ = await _index.AddDocumentsAsync(new []
        {
            BookmarkSearchable.FromBookmarkWithId(result, data)
        });
        
        return (result, data);
    }

    public async Task<Paginated<Bookmark>?> Search(string query, string? category, int page, int pageSize)
    {
        await _index.UpdateFilterableAttributesAsync(new[] {"categoryName"});
        
        var results = await _index.SearchAsync<BookmarkSearchable>(query, new SearchQuery
        {
            Q = query,
            Limit = pageSize,
            Offset = (page - 1) * pageSize,
            AttributesToCrop = new[] {"description"},
            CropLength = 128,
            Filter = category is not null ? new[]{$"categoryName = {category}"} : null,
        });

        if (results is null || results.NbHits == 0 || !results.Hits.Any())
            return null;

        var totalPages = (int) Math.Ceiling(results.NbHits / (double) pageSize);
        return new Paginated<Bookmark>
        {
            Content = results.Hits.Select(searchResult => new Bookmark
            {
                Id = searchResult.Id, Title = searchResult.Title, Description = searchResult.Description,
                ImageUrl = searchResult.ImageUrl, Url = searchResult.Url,
                ImageAlt = searchResult.ImageAlt, OgType = searchResult.OgType, VideoUrl = searchResult.VideoUrl,
                IsLinkBroken = searchResult.IsLinkBroken
            }),
            Page = page,
            ItemsPerPage = pageSize,
            TotalPages = totalPages,
            TotalItems = results.NbHits,
            HasNext = page < totalPages,
        };
    }

    public async Task<Paginated<BookmarkCategorized>?> GetPaginated(int page, int pageSize)
    {
        var itemCount = await _repository.GetTotalItems();

        if (itemCount == 0)
            return null;

        var entities = await _repository.GetPaginated(pageSize, (page - 1) * pageSize);

        if (entities is null)
            return null;

        var totalPages = (int) Math.Ceiling(itemCount / (double) pageSize);
        return new Paginated<BookmarkCategorized>
        {
            Content = entities.Select(entity => new BookmarkCategorized
            {
                Id = entity.Id, Title = entity.Title, Description = entity.Description,
                ImageUrl = entity.ImageUrl, Url = entity.Url, CategoryId = entity.CategoryId,
                ImageAlt = entity.ImageAlt, OgType = entity.OgType, VideoUrl = entity.VideoUrl,
                IsLinkBroken = entity.IsLinkBroken
            }),
            Page = page,
            ItemsPerPage = pageSize,
            TotalPages = totalPages,
            TotalItems = itemCount,
            HasNext = page < totalPages,
        };
    }
    
    public async Task<Paginated<BookmarkCategorized>?> GetPaginatedByCategory(string categoryId, int page, int pageSize)
    {
        var itemCount = await _repository.GetItemsCountByCategory(categoryId);

        if (itemCount == 0)
            return null;

        var entities = await _repository.GetPaginatedByCategory(categoryId, pageSize, (page - 1) * pageSize);

        if (entities is null)
            return null;

        var totalPages = (int) Math.Ceiling(itemCount / (double) pageSize);
        return new Paginated<BookmarkCategorized>
        {
            Content = entities.Select(entity => new BookmarkCategorized
            {
                Id = entity.Id, Title = entity.Title, Description = entity.Description,
                ImageUrl = entity.ImageUrl, Url = entity.Url, CategoryId = entity.CategoryId,
                ImageAlt = entity.ImageAlt, OgType = entity.OgType, VideoUrl = entity.VideoUrl,
                IsLinkBroken = entity.IsLinkBroken
            }),
            Page = page,
            ItemsPerPage = pageSize,
            TotalPages = totalPages,
            TotalItems = itemCount,
            HasNext = page < totalPages,
        };
    }

    public async Task<bool> SetCategory(string id, string categoryId)
    {
        var category = await _categoryRepository.GetById(categoryId);

        if (category is null)
            return false;
        
        var entity = await _repository.SetCategory(id, categoryId);

        if (entity is null)
            return false;

        var seachable = BookmarkSearchable.FromBookmarkEntity(entity);
        seachable.CategoryName = category.Title;

        var _ = await _index.AddDocumentsAsync(new []
        {
            seachable
        });

        return true;
    }
}