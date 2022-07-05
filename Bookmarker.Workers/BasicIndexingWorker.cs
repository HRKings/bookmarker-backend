using Bookmarker.Contracts.Base;
using Bookmarker.Data.Repositories;
using Bookmarker.Search;
using Index = Meilisearch.Index;

namespace Bookmarker.Workers;

public class BasicIndexingWorker: BackgroundService
{
    private readonly ILogger<BasicIndexingWorker> _logger;
    private readonly BookmarkIndexRepository _indexRepository;
    
    private readonly Index _index;
    
    public BasicIndexingWorker(ILogger<BasicIndexingWorker> logger, IServiceProvider serviceProvider, IndexWrapper<SearchBookmark> indexWrapper)
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

            var toIndex = await _indexRepository.GetBookmarksToBeFirstIndexed();

            if (toIndex.Count > 0)
            {
                _ = await _index.AddDocumentsInBatchesAsync(toIndex.Select(SearchBookmark.FromEntity), cancellationToken: stoppingToken);
            } 
            else
            {
                _logger.LogInformation("No bookmarks to basic index");
            }
            
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
}