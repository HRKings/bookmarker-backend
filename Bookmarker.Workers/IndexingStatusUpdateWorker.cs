using Bookmarker.Contracts.Base.Bookmark.Interfaces;
using Bookmarker.Contracts.Base.Bookmark.Search;
using Bookmarker.Search;
using Index = Meilisearch.Index;

namespace Bookmarker.Workers;

public class IndexingStatusUpdateWorker: BackgroundService
{
    private readonly ILogger<IndexingStatusUpdateWorker> _logger;
    private readonly IBookmarkIndexRepository _indexRepository;
    
    private readonly Index _index;
    
    public IndexingStatusUpdateWorker(ILogger<IndexingStatusUpdateWorker> logger, IBookmarkIndexRepository indexRepository, IndexWrapper<BookmarkSearchable> indexWrapper)
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

            var toUpdate = await _indexRepository.GetBookmarksIdToBeFirstIndexed();

            if (toUpdate is null)
            {
                _logger.LogInformation("No bookmarks to update the status");
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                continue;
            } 
            
            foreach (var id in toUpdate)
            {
                try
                {
                    var document = await _index.GetDocumentAsync<BookmarkSearchable>(id, stoppingToken);

                    if (document is null)
                        continue;

                    var rows = await _indexRepository.SetIndexStatus(id, document.CategoryName is not null,
                        document.ArticleContent is not null, document.PageContent is not null);

                    if (rows != 1)
                    {
                        _logger.LogError("Index status update failed for {Id}", id);
                        continue;
                    }

                    _logger.LogInformation("Index status updated for {Id}", id);
                }
                catch
                {
                    // ignored
                }
            }
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
}