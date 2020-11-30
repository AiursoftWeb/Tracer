using AngleSharp.Html.Dom;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;
using Tracer.Tests.Tools;
using static Aiursoft.WebTools.Extends;

namespace Tracer.Tests.IntegrationTests
{
    [TestClass]
    public class BasicTests
    {
        private readonly string _endpointUrl = $"http://localhost:{_port}";
        private const int _port = 15999;
        private IHost _server;
        private HttpClient _http;

        [TestInitialize]
        public async Task CreateServer()
        {
            _server = App<Startup>(port: _port);
            _http = new HttpClient();
            await _server.StartAsync();
        }

        [TestCleanup]
        public async Task CleanServer()
        {
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
            Assert.AreEqual("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
            var p = (IHtmlElement)doc.QuerySelector("p.lead");
            Assert.AreEqual("Aiursoft Tracer is a simple network quality testing app. Helps testing the connection speed between you and Aiursoft services.", p.InnerHtml);
        }

        [TestMethod]
        public async Task GetDownload()
        {
            var url = _endpointUrl + "/Home/Download";
            var response = await _http.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.AreEqual("application/octet-stream", response.Content.Headers.ContentType.ToString());
            Assert.AreEqual(1024 * 1024, content.Length);
        }

        [TestMethod]
        public async Task Ping()
        {
            var url = _endpointUrl + "/pINg";
            var response = await _http.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            Assert.AreEqual("[]", content);
        }
    }
}
