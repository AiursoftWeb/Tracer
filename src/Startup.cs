using Aiursoft.Directory.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.SDK;
using Aiursoft.WebTools.Models;

namespace Aiursoft.Tracer;

public class Startup : IWebStartup
{
    public virtual void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        services.AddAiurMvc();
        services
            .AddControllersWithViews()
            .AddApplicationPart(typeof(SDK.Views.Shared.Components.AiurHeader.AiurHeader).Assembly);
        services.AddAiursoftAppAuthentication(configuration.GetSection("AiursoftAuthentication"));
        services.AddAiursoftObserver(configuration.GetSection("AiursoftObserver"));
        services.AddAiursoftSdk();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiursoftHandler(env.IsDevelopment());
        app.UseWebSockets();
        app.UseAiursoftAppRouters();
    }
}