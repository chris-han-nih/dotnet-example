namespace queue_background_worker.Controllers;

using Microsoft.AspNetCore.Mvc;
using queue_background_worker.Worker;

[ApiController]
[Route("api/[controller]")]
public class PlayersController: ControllerBase
{
    private readonly ILogger<PlayersController> _logger;
    private readonly IBackgroundTaskQueue _taskQueue;

    public PlayersController(ILogger<PlayersController> logger, IBackgroundTaskQueue taskQueue)
    {
        _logger = logger;
        _taskQueue = taskQueue;
    }
    
    [HttpPost]
    public async Task<IActionResult> Post()
    {
        await _taskQueue.QueueBackgroundWorkItemAsync(PrintPlayer);
        return Ok();
    }
    
    private async ValueTask PrintPlayer(CancellationToken cancellationToken)
    {
        Thread.Sleep(TimeSpan.FromSeconds(2));
        _logger.LogInformation("Player created.{Player}", Guid.NewGuid().ToString());
    }
}