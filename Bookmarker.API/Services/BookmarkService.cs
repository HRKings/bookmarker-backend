using Bookmarker.Contracts.Base;
using Bookmarker.Contracts.Utils;
using Bookmarker.Data.Repositories;
using Bookmarker.Extraction;
using Bookmarker.Model.Entities;
using Bookmarker.Model.Utils;
using Bookmarker.Search;
using Meilisearch;
using Index = Meilisearch.Index;

namespace Bookmarker.API.Services;

public class BookmarkService
{
    private readonly BookmarkRepository _repository;
    private readonly CategoryRepository _categoryRepository;
    private readonly BookmarkIndexRepository _indexRepository;
    private readonly Index _index;
    


    public BookmarkService(BookmarkRepository repository, IndexWrapper<SearchBookmark> indexWrapper, CategoryRepository categoryRepository, BookmarkIndexRepository indexRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _indexRepository = indexRepository;
        _index = indexWrapper.Index;
    }

    public async Task<string?> CreateFull(BookmarkCreation data)
    {
        var result = await _repository.Create(data.ToEntity());

        if (result is null)
            return null;

        _ = await _index.AddDocumentsAsync(new []
        {
            SearchBookmark.FromEntity(result)
        });

        _ = await _indexRepository.Create(new BookmarkIndexStatus
        {
            Id = result.Id,
            HasBasicIndex = true,
        });
        
        return result.Id.ToString();
    }

    public async Task<Bookmark?> CreateWithRequestFallback(BookmarkCreation data)
    {
        var dataEntity = data.ToEntity();
        var parsedData = await OpenGraphParser.GetOpenGraph(data.Url) ?? new Bookmark();

        foreach (var property in parsedData.GetType().GetProperties())
        {
            if(property.GetValue(parsedData) is null)
                property.SetValue(parsedData, property.GetValue(dataEntity));
        }
        
        var result  = await _repository.Create(parsedData);
        
        if (result is null)
            return null;
        
        _ = await _index.AddDocumentsAsync(new []
        {
            SearchBookmark.FromEntity(result)
        });
        
        _ = await _indexRepository.Create(new BookmarkIndexStatus
        {
            Id = result.Id,
            HasBasicIndex = true,
        });
        
        return result;
    }
    
    public async Task<Bookmark?> CreateWithParsedFallback(BookmarkCreation data)
    {
        var parsedData = await OpenGraphParser.GetOpenGraph(data.Url) ?? new Bookmark();
        
        foreach (var property in data.GetType().GetProperties())
        {
            if(property.GetValue(data) is null)
                property.SetValue(data, property.GetValue(parsedData));
        }
        
        var result  = await _repository.Create(data.ToEntity());
        
        if (result is null)
            return null;

        var category = await _categoryRepository.GetById(result.CategoryId.ToString() ?? "54453511-6970-4fb7-b3a4-51714ce5fd35");
        result.Category = category;
        
        _ = await _index.AddDocumentsAsync(new []
        {
            SearchBookmark.FromEntity(result)
        });
        
        _ = await _indexRepository.Create(new BookmarkIndexStatus
        {
            Id = result.Id,
            HasBasicIndex = true,
        });
        
        return result;
    }

    public async Task<Paginated<Bookmark>?> Search(string query, string? category, int page, int pageSize)
    {
        await _index.UpdateFilterableAttributesAsync(new[] {"categoryName"});
        
        var results = await _index.SearchAsync<SearchBookmark>(query, new SearchQuery
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
                Id = searchResult.Id.ToGuid(), Title = searchResult.Title, Description = searchResult.Description,
                ImageUrl = searchResult.ImageUrl, Url = searchResult.Url,
                ImageAlt = searchResult.ImageAlt, OgType = searchResult.OgType, VideoUrl = searchResult.VideoUrl,
            }),
            Page = page,
            ItemsPerPage = pageSize,
            TotalPages = totalPages,
            TotalItems = results.NbHits,
            HasNext = page < totalPages,
        };
    }

    public async Task<Paginated<Bookmark>?> GetPaginated(int page, int pageSize)
    {
        var itemCount = await _repository.GetTotalItems();

        if (itemCount == 0)
            return null;

        var entities = await _repository.GetPaginated(pageSize, (page - 1) * pageSize);

        if (entities.Count == 0)
            return null;
        
        var totalPages = (int) Math.Ceiling(itemCount / (double) pageSize);
        return new Paginated<Bookmark>
        {
            Content = entities,
            Page = page,
            ItemsPerPage = pageSize,
            TotalPages = totalPages,
            TotalItems = itemCount,
            HasNext = page < totalPages,
        };
    }
    
    public async Task<Paginated<Bookmark>?> GetPaginatedByCategory(string categoryId, int page, int pageSize)
    {
        var itemCount = await _repository.GetItemsCountByCategory(categoryId);

        if (itemCount == 0)
            return null;

        var entities = await _repository.GetPaginatedByCategory(categoryId, pageSize, (page - 1) * pageSize);

        if (entities.Count == 0)
            return null;

        var totalPages = (int) Math.Ceiling(itemCount / (double) pageSize);
        return new Paginated<Bookmark>
        {
            Content = entities,
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

        entity.Category = category;
        var searchable = SearchBookmark.FromEntity(entity);
        searchable.CategoryName = category.Title;

        _ = await _index.AddDocumentsAsync(new []
        {
            searchable
        });

        return true;
    }
}