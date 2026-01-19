namespace Aiursoft.Tracer.Tests.IntegrationTests;

[TestClass]
public class ErrorControllerTests : TestBase
{
    [TestMethod]
    public async Task GetError()
    {
        var url = "/Error/Code500";
        var response = await Http.GetAsync(url);
        response.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public async Task GetUnauthorized()
    {
        var url = "/Error/Code403?returnUrl=/dashboard";
        var response = await Http.GetAsync(url);
        response.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public async Task GetCode400()
    {
        var url = "/Error/Code400";
        var response = await Http.GetAsync(url);
        response.EnsureSuccessStatusCode();
    }
}
