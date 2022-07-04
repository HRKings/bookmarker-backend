using Bookmarker.Contracts.Base.Bookmark.Interfaces;

namespace Bookmarker.Workers;

public class DuplicatedLinkWorker: BackgroundService
{
    private readonly ILogger<BrokenLinkWorker> _logger;
    private readonly IBookmarkRepository _repository;
    
    public DuplicatedLinkWorker(ILogger<BrokenLinkWorker> logger, IBookmarkRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

            var toCheck = await _repository.GetActiveIdsAndPureUrls();

            if (toCheck is not null)
            {
                foreach (var (id, pureUrl) in toCheck)
                {
                    var duplicates = await _repository.GetDuplicatedLinks(id, pureUrl);
                    if(duplicates is null)
                        continue;

                    foreach (var duplicate in duplicates)
                        await _repository.FlagDuplicatedLink(id, duplicate);
                }
            } 
            else
            {
                _logger.LogInformation("No bookmarks to check");
            }
            
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}