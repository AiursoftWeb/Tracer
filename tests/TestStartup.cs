using Microsoft.Extensions.Configuration;

namespace Tracer.Tests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration) { }
    }
}
