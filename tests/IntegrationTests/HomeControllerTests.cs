namespace Aiursoft.Tracer.Tests.IntegrationTests;

[TestClass]
public class HomeControllerTests : TestBase
{
    [TestMethod]
    public async Task GetIndex()
    {
        var url = "/";
        var response = await Http.GetAsync(url);
        response.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public async Task GetIp()
    {
        var url = "/ip";
        var response = await Http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.IsFalse(string.IsNullOrWhiteSpace(content));
    }
}
