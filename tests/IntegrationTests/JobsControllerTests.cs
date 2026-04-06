namespace Aiursoft.Tracer.Tests.IntegrationTests;

[TestClass]
public class JobsControllerTests : TestBase
{
    [TestMethod]
    public async Task TestJobsWorkflow()
    {
        await LoginAsAdmin();

        // 1. Index
        var indexResponse = await Http.GetAsync("/Jobs/Index");
        indexResponse.EnsureSuccessStatusCode();

        // 2. Trigger DummyJob
        var triggerAResponse = await PostForm("/Jobs/Trigger", new Dictionary<string, string>
        {
            { "jobTypeName", "DummyJob" }
        });
        AssertRedirect(triggerAResponse, "/Jobs");

        // 3. Trigger OrphanAvatarCleanupJob
        var triggerBResponse = await PostForm("/Jobs/Trigger", new Dictionary<string, string>
        {
            { "jobTypeName", "OrphanAvatarCleanupJob" }
        });
        AssertRedirect(triggerBResponse, "/Jobs");

        // 4. Index again (check if jobs are listed)
        var indexResponse2 = await Http.GetAsync("/Jobs/Index");
        indexResponse2.EnsureSuccessStatusCode();

        // 5. Cancel with a dummy ID — endpoint should still redirect gracefully
        var cancelResponse = await PostForm("/Jobs/Cancel", new Dictionary<string, string>
        {
            { "jobId", Guid.NewGuid().ToString() }
        });
        AssertRedirect(cancelResponse, "/Jobs");
    }
}
