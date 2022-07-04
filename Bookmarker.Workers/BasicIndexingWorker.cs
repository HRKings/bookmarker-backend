using Bookmarker.Contracts.Base.Bookmark.Interfaces;
using Bookmarker.Contracts.Base.Bookmark.Search;
using Bookmarker.Search;
using Index = Meilisearch.Index;

namespace Bookmarker.Workers;

public class BasicIndexingWorker: BackgroundService
{
    private readonly ILogger<BasicIndexingWorker> _logger;
    private readonly IBookmarkIndexRepository _indexRepository;
    
    private readonly Index _index;
    
    public BasicIndexingWorker(ILogger<BasicIndexingWorker> logger, IBookmarkIndexRepository indexRepository, IndexWrapper<BookmarkSearchable> indexWrapper)
    {
        _logger = logger;
        _indexRepository = indexRepository;
        _index = indexWrapper.Index;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

            var toIndex = await _indexRepository.GetBookmarksToBeFirstIndexed();

            if (toIndex is not null)
            {
                _ = await _index.AddDocumentsInBatchesAsync(toIndex.Select(unit =>
                {
                    var indexable = BookmarkSearchable.FromBookmarkWithId(unit.Id, unit);

                    indexable.CategoryName = unit.CategoryName;

                    return indexable;
                }), cancellationToken: stoppingToken);
            } 
            else
            {
                _logger.LogInformation("No bookmarks to basic index");
            }
            
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
}