using Microsoft.Extensions.Configuration;

namespace Aiursoft.Tracer.Tests;

public class TestStartup : Startup
{
    public TestStartup(IConfiguration configuration) : base(configuration)
    {
    }
}