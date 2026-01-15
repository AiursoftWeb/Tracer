using Aiursoft.UiStack.Layout;

namespace Aiursoft.Tracer.Models.BackgroundJobs;

public class JobsIndexViewModel : UiStackLayoutViewModel
{
    public IEnumerable<JobInfo> AllRecentJobs { get; init; } = [];
}
