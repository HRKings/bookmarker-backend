using Bookmarker.Contracts.Base.Bookmark.Interfaces;

namespace Bookmarker.Workers;

public class BrokenLinkWorker: BackgroundService
{
    private readonly ILogger<BrokenLinkWorker> _logger;
    private readonly IBookmarkRepository _repository;
    
    public BrokenLinkWorker(ILogger<BrokenLinkWorker> logger, IBookmarkRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const bool CHECK_FOR_BROKEN_LINKS = false;
        while (CHECK_FOR_BROKEN_LINKS && !stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

            var toCheck = await _repository.GetActiveIdsAndUrls();

            if (toCheck is not null)
            {
                var httpClient = new HttpClient();

                foreach (var (id, url) in toCheck)
                {
                    var request = await httpClient.GetAsync(url, stoppingToken);

                    if (!request.IsSuccessStatusCode)
                        await _repository.FlagBrokenLink(id);
                }
            } 
            else
            {
                _logger.LogInformation("No bookmarks to basic index");
            }
            
            await Task.Delay(TimeSpan.FromDays(30), stoppingToken);
        }
    }
}