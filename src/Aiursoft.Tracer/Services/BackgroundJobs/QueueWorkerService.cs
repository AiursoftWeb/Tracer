using Aiursoft.Tracer.Models.BackgroundJobs;

namespace Aiursoft.Tracer.Services.BackgroundJobs;

/// <summary>
/// Background service that processes jobs from the CanonQueue.
/// </summary>
public class QueueWorkerService(
    BackgroundJobQueue backgroundJobQueue,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<QueueWorkerService> logger) : IHostedService, IDisposable
{
    private Timer? _timer;
    private Timer? _cleanupTimer;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Queue Worker Service is starting");

        // Process jobs every 100ms
        _timer = new Timer(ProcessJobs, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));

        // Cleanup old jobs every 5 minutes
        _cleanupTimer = new Timer(CleanupOldJobs, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5));

        return Task.CompletedTask;
    }

    private void ProcessJobs(object? state)
    {
        // Try to acquire the semaphore (non-blocking)
        if (!_semaphore.Wait(0))
        {
            return; // Already processing
        }

        try
        {
            var queues = backgroundJobQueue.GetQueuesWithPendingJobs().ToList();

            foreach (var queueName in queues)
            {
                // Try to get next job for this queue (will return null if queue is already processing)
                var job = backgroundJobQueue.TryDequeueNextJob(queueName);
                if (job != null)
                {
                    // Process job asynchronously without blocking the timer
                    _ = Task.Run(async () => await ProcessJobAsync(job));
                }
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task ProcessJobAsync(JobInfo job)
    {
        try
        {
            logger.LogInformation("Processing job {JobId} ({JobName}) from queue {QueueName}",
                job.JobId, job.JobName, job.QueueName);

            // Create a scope for dependency injection
            using var scope = serviceScopeFactory.CreateScope();

            // Resolve the service
            var service = scope.ServiceProvider.GetRequiredService(job.ServiceType);

            // Execute the job
            await job.JobAction(service);

            // Mark as success
            backgroundJobQueue.CompleteJob(job.JobId, true);

            logger.LogInformation("Job {JobId} ({JobName}) completed successfully",
                job.JobId, job.JobName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Job {JobId} ({JobName}) failed with error: {Error}",
                job.JobId, job.JobName, ex.Message);

            // Mark as failed with error message
            backgroundJobQueue.CompleteJob(job.JobId, false, ex.ToString());
        }
    }

    private void CleanupOldJobs(object? state)
    {
        try
        {
            logger.LogInformation("Cleaning up old jobs");
            backgroundJobQueue.CleanupOldJobs(TimeSpan.FromHours(1));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error cleaning up old jobs");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Queue Worker Service is stopping");

        _timer?.Change(Timeout.Infinite, 0);
        _cleanupTimer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _cleanupTimer?.Dispose();
        _semaphore.Dispose();
        GC.SuppressFinalize(this);
    }
}
