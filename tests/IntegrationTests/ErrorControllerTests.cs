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

    [TestMethod]
    [DataRow("safe=1")]
    [DataRow("safe%5B=1")]
    public async Task UnknownRouteWithFormBodyRemainsNotFound(string body)
    {
        using var content = new StringContent(body, null, "application/x-www-form-urlencoded");
        using var response = await Http.PostAsync("/error-page-regression/missing/route/extra", content);
        Assert.AreEqual(404, (int)response.StatusCode);
    }

    [TestMethod]
    [DataRow(400)]
    [DataRow(403)]
    [DataRow(404)]
    [DataRow(500)]
    public async Task ErrorPageIgnoresMalformedFormAndUsesQueryReturnUrl(int code)
    {
        using var content = new StringContent("safe%5B=1&returnUrl=%2Fform-supplied&code=999", null,
            "application/x-www-form-urlencoded");
        using var response = await Http.PostAsync($"/Error/Code{code}?returnUrl=%2Fdashboard", content);
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        StringAssert.Contains(html, "href=\"/dashboard\"");
        Assert.IsFalse(html.Contains("href=\"/form-supplied\"", StringComparison.Ordinal));
    }
}
