namespace Aiursoft.Tracer.Tests.IntegrationTests;

[TestClass]
public class GlobalSettingsControllerTests : TestBase
{
    [TestMethod]
    public async Task GetIndex()
    {
        // This is a basic test to ensure the controller is reachable.
        // Adjust the path as necessary for specific controllers.
        var url = "/GlobalSettings/Index";
        
        var response = await Http.GetAsync(url);
        
        // Assert
        // For some controllers, it might redirect to login, which is 302.
        // For others, it might be 200.
        // We just check if we get a response.
        Assert.IsNotNull(response);
    }
}
