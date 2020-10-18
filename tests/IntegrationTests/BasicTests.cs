using System.Threading.Tasks;
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
            var content = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
            Assert.Contains("simple network quality testing app", content);
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
