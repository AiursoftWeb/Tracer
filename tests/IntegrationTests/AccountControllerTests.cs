namespace Aiursoft.Tracer.Tests.IntegrationTests;

[TestClass]
public class AccountControllerTests : TestBase
{
    [TestMethod]
    public async Task GetLogin()
    {
        var url = "/Account/Login";
        var response = await Http.GetAsync(url);
        response.EnsureSuccessStatusCode();
    }
}
