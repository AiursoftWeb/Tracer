using Aiursoft.Directory.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.SDK;
using Aiursoft.WebTools.Models;

namespace Aiursoft.Tracer;

public class Startup : IWebStartup
{
    public void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        services.AddAiursoftWebFeatures();
        services.AddAiursoftAppAuthentication(configuration.GetSection("AiursoftAuthentication"));
        services.AddAiursoftObserver(configuration.GetSection("AiursoftObserver"));
        services.AddScannedServices();
    }

    public void Configure(WebApplication app)
    {
        app.UseAiursoftHandler(app.Environment.IsDevelopment());
        app.UseWebSockets();
        app.UseAiursoftAppRouters();
    }
}