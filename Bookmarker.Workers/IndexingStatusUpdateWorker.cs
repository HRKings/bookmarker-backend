using Bookmarker.Contracts.Base;
using Bookmarker.Data.Repositories;
using Bookmarker.Search;
using Index = Meilisearch.Index;

namespace Bookmarker.Workers;

public class IndexingStatusUpdateWorker: BackgroundService
{
    private readonly ILogger<IndexingStatusUpdateWorker> _logger;
    private readonly BookmarkIndexRepository _indexRepository;
    
    private readonly Index _index;
    
    public IndexingStatusUpdateWorker(ILogger<IndexingStatusUpdateWorker> logger, IServiceProvider serviceProvider, IndexWrapper<SearchBookmark> indexWrapper)
    {
        _logger = logger;
        _index = indexWrapper.Index;
        
        var scope = serviceProvider.CreateScope();
        _indexRepository = scope.ServiceProvider.GetService<BookmarkIndexRepository>()!;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

            var toUpdate = await _indexRepository.GetBookmarksIdToBeFirstIndexed();

            if (toUpdate.Count > 0)
            {
                foreach (var id in toUpdate)
                {
                    try
                    {
                        var document = await _index.GetDocumentAsync<SearchBookmark>(id, stoppingToken);

                        if (document is null)
                            continue;

                        var rows = await _indexRepository.SetIndexStatus(id, document.CategoryName is not null,
                            document.ArticleContent is not null, document.PageContent is not null);

                        if (!rows)
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
            }
            
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
}