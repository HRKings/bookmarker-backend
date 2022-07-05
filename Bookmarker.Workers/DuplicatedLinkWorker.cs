using Bookmarker.Data.Repositories;

namespace Bookmarker.Workers;

public class DuplicatedLinkWorker: BackgroundService
{
    private readonly ILogger<DuplicatedLinkWorker> _logger;
    private readonly BookmarkRepository _repository;
    
    public DuplicatedLinkWorker(ILogger<DuplicatedLinkWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        
        var scope = serviceProvider.CreateScope();
        _repository = scope.ServiceProvider.GetService<BookmarkRepository>()!;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

            var toCheck = await _repository.GetActives();

            if (toCheck.Count > 0)
            {
                foreach (var bookmark in toCheck)
                {
                    if (bookmark.PureUrl is null) 
                        continue;

                    var id = bookmark.Id.ToString();
                    var duplicates = _repository.GetDuplicatedLinks(id, bookmark.PureUrl);

                    await foreach (var duplicate in duplicates.WithCancellation(stoppingToken))
                        await _repository.FlagDuplicatedLink(id, duplicate.Id.ToString());
                }
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}