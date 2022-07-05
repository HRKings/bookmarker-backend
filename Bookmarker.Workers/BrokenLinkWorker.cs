using Bookmarker.Data.Repositories;

namespace Bookmarker.Workers;

public class BrokenLinkWorker: BackgroundService
{
    private readonly ILogger<BrokenLinkWorker> _logger;
    private readonly BookmarkRepository _repository;
    
    public BrokenLinkWorker(ILogger<BrokenLinkWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        
        var scope = serviceProvider.CreateScope();
        _repository = scope.ServiceProvider.GetService<BookmarkRepository>()!;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const bool CHECK_FOR_BROKEN_LINKS = false;
        while (CHECK_FOR_BROKEN_LINKS &&  !stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

            var toCheck = await _repository.GetActives();
            
            if (toCheck.Count > 0)
            {
                var httpClient = new HttpClient();

                foreach (var bookmark in toCheck)
                {
                    var request = await httpClient.GetAsync(bookmark.Url, stoppingToken);

                    if (!request.IsSuccessStatusCode)
                        await _repository.FlagBrokenLink(bookmark.Id.ToString());
                }
            }

            await Task.Delay(TimeSpan.FromDays(30), stoppingToken);
        }
    }
}