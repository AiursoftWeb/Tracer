using System.Collections.Concurrent;
using Aiursoft.Tracer.Models.BackgroundJobs;

namespace Aiursoft.Tracer.Services.BackgroundJobs;

/// <summary>
/// A queue system for background jobs with dependency injection support.
/// Each queue processes jobs sequentially, but different queues can process jobs in parallel.
/// </summary>
public class BackgroundJobQueue
{
    private readonly ConcurrentDictionary<Guid, JobInfo> _allJobs = new();
    private readonly ConcurrentDictionary<string, ConcurrentQueue<Guid>> _queuesByName = new();
    private readonly ConcurrentDictionary<string, bool> _queueProcessingStatus = new();

    /// <summary>
    /// Queue a job with dependency injection support.
    /// </summary>
    /// <typeparam name="TService">The type of service to inject.</typeparam>
    /// <param name="queueName">The name of the queue. Use a unique value (like a GUID) to execute immediately.</param>
    /// <param name="jobName">A descriptive name for the job.</param>
    /// <param name="job">The function to execute with the injected service.</param>
    /// <returns>The job ID.</returns>
    public Guid QueueWithDependency<TService>(string queueName, string jobName, Func<TService, Task> job)
        where TService : notnull
    {
        var jobInfo = new JobInfo
        {
            QueueName = queueName,
            JobName = jobName,
            ServiceType = typeof(TService),
            JobAction = async (service) => await job((TService)service)
        };

        _allJobs[jobInfo.JobId] = jobInfo;

        var queue = _queuesByName.GetOrAdd(queueName, _ => new ConcurrentQueue<Guid>());
        queue.Enqueue(jobInfo.JobId);

        return jobInfo.JobId;
    }

    /// <summary>
    /// Queue a job with dependency injection support using default queue name based on service type.
    /// </summary>
    public Guid QueueWithDependency<TService>(Func<TService, Task> job)
        where TService : notnull
    {
        return QueueWithDependency(typeof(TService).Name, typeof(TService).Name, job);
    }

    /// <summary>
    /// Get all jobs (for UI display).
    /// </summary>
    public IEnumerable<JobInfo> GetAllJobs()
    {
        return _allJobs.Values.OrderByDescending(j => j.QueuedAt);
    }

    /// <summary>
    /// Get jobs completed in the last hour (successful or failed).
    /// </summary>
    public IEnumerable<JobInfo> GetRecentCompletedJobs(TimeSpan within)
    {
        var cutoff = DateTime.UtcNow - within;
        return _allJobs.Values
            .Where(j => j.CompletedAt.HasValue && j.CompletedAt.Value >= cutoff)
            .OrderByDescending(j => j.CompletedAt);
    }

    /// <summary>
    /// Get pending jobs.
    /// </summary>
    public IEnumerable<JobInfo> GetPendingJobs()
    {
        return _allJobs.Values
            .Where(j => j.Status == JobStatus.Pending)
            .OrderBy(j => j.QueuedAt);
    }

    /// <summary>
    /// Get processing jobs.
    /// </summary>
    public IEnumerable<JobInfo> GetProcessingJobs()
    {
        return _allJobs.Values
            .Where(j => j.Status == JobStatus.Processing)
            .OrderBy(j => j.StartedAt);
    }

    /// <summary>
    /// Cancel a pending job.
    /// </summary>
    /// <returns>True if cancelled, false if job is not pending.</returns>
    public bool CancelJob(Guid jobId)
    {
        if (_allJobs.TryGetValue(jobId, out var job) && job.Status == JobStatus.Pending)
        {
            job.Status = JobStatus.Cancelled;
            job.CompletedAt = DateTime.UtcNow;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Try to get the next job to process for a given queue.
    /// Returns null if queue is already processing or no jobs available.
    /// </summary>
    internal JobInfo? TryDequeueNextJob(string queueName)
    {
        // Check if this queue is already processing
        if (_queueProcessingStatus.TryGetValue(queueName, out var isProcessing) && isProcessing)
        {
            return null;
        }

        // Try to get the queue
        if (!_queuesByName.TryGetValue(queueName, out var queue))
        {
            return null;
        }

        // Try to dequeue jobs until we find a non-cancelled one
        while (queue.TryDequeue(out var jobId))
        {
            if (_allJobs.TryGetValue(jobId, out var job))
            {
                // Skip cancelled jobs
                if (job.Status == JobStatus.Cancelled)
                {
                    continue;
                }

                // Mark this queue as processing
                _queueProcessingStatus[queueName] = true;

                // Update job status
                job.Status = JobStatus.Processing;
                job.StartedAt = DateTime.UtcNow;

                return job;
            }
        }

        return null;
    }

    /// <summary>
    /// Mark a job as completed (success or failed).
    /// </summary>
    internal void CompleteJob(Guid jobId, bool success, string? errorMessage = null)
    {
        if (_allJobs.TryGetValue(jobId, out var job))
        {
            job.Status = success ? JobStatus.Success : JobStatus.Failed;
            job.CompletedAt = DateTime.UtcNow;
            job.ErrorMessage = errorMessage;

            // Mark the queue as no longer processing
            _queueProcessingStatus[job.QueueName] = false;
        }
    }

    /// <summary>
    /// Get all queue names that have pending jobs.
    /// </summary>
    internal IEnumerable<string> GetQueuesWithPendingJobs()
    {
        return _queuesByName.Keys;
    }

    /// <summary>
    /// Clean up old completed jobs.
    /// </summary>
    public void CleanupOldJobs(TimeSpan olderThan)
    {
        var cutoff = DateTime.UtcNow - olderThan;
        var jobsToRemove = _allJobs.Values
            .Where(j => j.CompletedAt.HasValue && j.CompletedAt.Value < cutoff)
            .Select(j => j.JobId)
            .ToList();

        foreach (var jobId in jobsToRemove)
        {
            _allJobs.TryRemove(jobId, out _);
        }
    }
}
