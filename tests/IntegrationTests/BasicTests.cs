using Aiursoft.CSTools.Tools;
using AngleSharp.Html.Dom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aiursoft.Tracer.Tests.Tools;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;
using Aiursoft.WebTools.Services;
using Aiursoft.AiurObserver;

namespace Aiursoft.Tracer.Tests.IntegrationTests;

[TestClass]
public class BasicTests
{
    private readonly string _endpointUrl;
    private readonly int _port;
    private readonly HttpClient _http;
    private IHost? _server;

    public BasicTests()
    {
        _port = Network.GetAvailablePort();
        _endpointUrl = $"http://localhost:{_port}";
        _http = new HttpClient();
    }

    [TestInitialize]
    public async Task CreateServer()
    {
        _server = App<Startup>(Array.Empty<string>(), port: _port);
        await _server.StartAsync();
    }

    [TestCleanup]
    public async Task CleanServer()
    {
        if (_server == null) return;
        await _server.StopAsync();
        _server.Dispose();
    }

    [TestMethod]
    [DataRow("/")]
    [DataRow("/hOmE?aaaaaa=bbbbbb")]
    [DataRow("/hOmE/InDeX")]
    public async Task GetHome(string url)
    {
        var response = await _http.GetAsync(_endpointUrl + url);
        var doc = await HtmlHelpers.GetDocumentAsync(response);

        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.AreEqual("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        var p = doc.QuerySelector("p.lead") as IHtmlElement;
        Assert.AreEqual(
            "Aiursoft Tracer is a simple network quality testing app. Helps testing the connection speed between you and Aiursoft services.",
            p?.InnerHtml);
    }

    [TestMethod]
    public async Task GetDownload()
    {
        var url = _endpointUrl + "/Home/Download";
        var response = await _http.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.AreEqual("application/octet-stream", response.Content.Headers.ContentType?.ToString());
        Assert.IsTrue(1024 * 1024 * 900 < content.Length);
    }

    [TestMethod]
    public async Task Ping()
    {
        var url = _endpointUrl + "/pINg";
        var response = await _http.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        Assert.AreEqual("[]", content);
    }

    [TestMethod]
    public async Task TestConnect()
    {
        var messagesCount = 0;
        var endPoint = _endpointUrl.Replace("http", "ws") + "/Home/Pushing";
        var socket = await endPoint.ConnectAsWebSocketServer();
        await Task.Factory.StartNew(() => socket.Listen());

        socket.Subscribe(_ => 
        {
            messagesCount++;
            return Task.CompletedTask;
        });
        await Task.Delay(5000);
        await socket.Close();
        await Task.Delay(10);

        var latestTime = socket.LastMessage.Split('|')[0];
        Assert.IsTrue(DateTime.TryParse(latestTime, out _), $"Got message {latestTime} is not a date time.");
        Assert.IsTrue(messagesCount > 30);
        Assert.IsTrue(messagesCount < 70);
    }
}