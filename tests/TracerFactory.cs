using Aiursoft.Archon.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.Scanner;
using Aiursoft.SDK;
using Aiursoft.SDK.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Tracer.Tests
{
    public class TracerFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services => 
            {
                services.AddLibraryDependencies();
            });
        }
    }
}
