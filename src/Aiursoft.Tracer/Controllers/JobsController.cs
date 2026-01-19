using Aiursoft.UiStack.Navigation;
using Aiursoft.WebTools.Attributes;
using Aiursoft.Tracer.Authorization;
using Aiursoft.Tracer.Models.BackgroundJobs;
using Aiursoft.Tracer.Services.BackgroundJobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Tracer.Services;

namespace Aiursoft.Tracer.Controllers;

/// <summary>
/// Controller for managing background jobs.
/// </summary>
[Authorize]
[LimitPerMin]
public class JobsController(BackgroundJobQueue backgroundJobQueue) : Controller
{
    [Authorize(Policy = AppPermissionNames.CanViewBackgroundJobs)]
    [RenderInNavBar(
        NavGroupName = "Administration",
        NavGroupOrder = 9999,
        CascadedLinksGroupName = "System",
        CascadedLinksIcon = "cog",
        CascadedLinksOrder = 9999,
        LinkText = "Background Jobs",
        LinkOrder = 2)]
    public IActionResult Index()
    {
        var oneHourAgo = TimeSpan.FromHours(1);
        var recentCompleted = backgroundJobQueue.GetRecentCompletedJobs(oneHourAgo);
        var pending = backgroundJobQueue.GetPendingJobs();
        var processing = backgroundJobQueue.GetProcessingJobs();

        // Merge all jobs and sort by queued time descending (newest first)
        var allJobs = pending
            .Concat(processing)
            .Concat(recentCompleted)
            .OrderByDescending(j => j.QueuedAt)
            .ToList();

        var viewModel = new JobsIndexViewModel
        {
            AllRecentJobs = allJobs
        };

        return this.StackView(viewModel);
    }

    [HttpPost]
    [Authorize(Policy = AppPermissionNames.CanViewBackgroundJobs)]
    [ValidateAntiForgeryToken]
    public IActionResult CreateTestJobA()
    {
        return CreateTestJob("Queue A", "Job A");
    }

    [HttpPost]
    [Authorize(Policy = AppPermissionNames.CanViewBackgroundJobs)]
    [ValidateAntiForgeryToken]
    public IActionResult CreateTestJobB()
    {
        return CreateTestJob("Queue B", "Job B");
    }

    private IActionResult CreateTestJob(string queueName, string jobPrefix)
    {
        // Queue a test job that sleeps for 15-30 seconds and has 10% chance of failure
        backgroundJobQueue.QueueWithDependency<ILogger<JobsController>>(
            queueName: queueName,
            jobName: $"{jobPrefix} {DateTime.UtcNow:HH:mm:ss}",
            job: async (logger) =>
            {
                var sleepSeconds = Random.Shared.Next(15, 31); // Random 15-30 seconds
                logger.LogInformation("Test job started, sleeping for {SleepSeconds} seconds...", sleepSeconds);
                await Task.Delay(TimeSpan.FromSeconds(sleepSeconds));

                // 10% chance of failure
                if (Random.Shared.Next(0, 100) < 10)
                {
                    logger.LogError("Test job intentionally failed!");
                    throw new Exception("Random test failure (10% chance)");
                }

                logger.LogInformation("Test job completed successfully!");
            });

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = AppPermissionNames.CanViewBackgroundJobs)]
    [ValidateAntiForgeryToken]
    public IActionResult Cancel(Guid jobId)
    {
        backgroundJobQueue.CancelJob(jobId);
        return RedirectToAction(nameof(Index));
    }
}
