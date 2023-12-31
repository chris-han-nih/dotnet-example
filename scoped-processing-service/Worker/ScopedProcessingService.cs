namespace scoped_processing_service.Worker;

public class ScopedProcessingService: IScopedProcessingService
{
    private int executionCount = 0;
    private readonly ILogger<ScopedProcessingService> _logger;

    public ScopedProcessingService(ILogger<ScopedProcessingService> logger) => _logger = logger;

    public async Task DoWork(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            executionCount++;

            _logger.LogInformation(
                "Scoped Processing Service is working. Count: {Count}", executionCount);

            await Task.Delay(10000, stoppingToken);
        }
    }
}