using Aiursoft.Scanner;
using System.Reflection;
using Aiursoft.WebTools.Abstractions.Models;

namespace Aiursoft.Tracer;

public class Startup : IWebStartup
{
    public void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        services.AddLibraryDependencies();

        services
            .AddControllersWithViews()
            .AddApplicationPart(typeof(Startup).Assembly);
    }

    public void Configure(WebApplication app)
    {
        app.UseStaticFiles();
        app.UseRouting();
        app.MapDefaultControllerRoute();
        app.UseWebSockets();
    }
}