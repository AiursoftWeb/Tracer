using System.Net;

namespace Aiursoft.Tracer.Tests.IntegrationTests;

[TestClass]
public class CultureControllerTests : TestBase
{
    [TestMethod]
    public async Task SetCulture()
    {
        var url = "/Culture/Set?culture=en&returnUrl=/";
        var response = await Http.GetAsync(url);
        
        // Assert
        Assert.AreEqual(HttpStatusCode.Found, response.StatusCode);
    }
}
