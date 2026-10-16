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

    [TestMethod]
    [DataRow("zh-CN&returnUrl=/Tips/Detail/8")]
    [DataRow("not/a/culture")]
    public async Task SetCultureInvalid(string culture)
    {
        var response = await Http.GetAsync($"/Culture/Set?culture={Uri.EscapeDataString(culture)}&returnUrl=/");

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsFalse(response.Headers.Contains("Set-Cookie"));
    }

}
