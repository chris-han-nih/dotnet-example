namespace queue_background_worker.Worker;

public interface IBackgroundTaskQueue
{
   ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem); 
   ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}