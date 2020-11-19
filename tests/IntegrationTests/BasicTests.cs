using AngleSharp.Html.Dom;
using System.Threading.Tasks;
using Tracer.Tests.Tools;
using Xunit;

namespace Tracer.Tests.IntegrationTests
{
    public class BasicTests : IClassFixture<TracerFactory<Startup>>
    {
        private readonly TracerFactory<Startup> _factory;

        public BasicTests(TracerFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/hOmE?aaaaaa=bbbbbb")]
        [InlineData("/hOmE/InDeX")]
        public async Task GetHome(string url)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync(url);
            var doc = await HtmlHelpers.GetDocumentAsync(response);

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
            var p = (IHtmlElement)doc.QuerySelector("p.lead");
            Assert.Equal("Aiursoft Tracer is a simple network quality testing app. Helps testing the connection speed between you and Aiursoft services.", p.InnerHtml);
        }

        [Theory]
        [InlineData("/home/download")]
        public async Task GetDownload(string url)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/octet-stream", response.Content.Headers.ContentType.ToString());
            Assert.Equal(1024 * 1024, content.Length);
        }
    }
}
