namespace scoped_processing_service.Worker;

public interface IScopedProcessingService
{
    Task DoWork(CancellationToken stoppingToken);
}