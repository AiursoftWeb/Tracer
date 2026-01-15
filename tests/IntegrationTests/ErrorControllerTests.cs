namespace Aiursoft.Tracer.Tests.IntegrationTests;

[TestClass]
public class ErrorControllerTests : TestBase
{
    [TestMethod]
    public async Task GetError()
    {
        var url = "/Error/Error";
        var response = await Http.GetAsync(url);
        response.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public async Task GetUnauthorized()
    {
        var url = "/Error/Unauthorized?returnUrl=/dashboard";
        var response = await Http.GetAsync(url);
        response.EnsureSuccessStatusCode();
    }
}
