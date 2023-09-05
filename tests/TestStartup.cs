using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Tracer.Tests;

public class TestStartup : Startup
{
    public override void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        base.ConfigureServices(configuration, environment, services);
        services
            .AddControllersWithViews()
            .AddApplicationPart(typeof(SDK.Views.Shared.Components.AiurHeader.AiurHeader).Assembly);
    }
}