using Aiursoft.Directory.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.SDK;

namespace Aiursoft.Tracer;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAiurMvc();
        services.AddAiursoftAppAuthentication(Configuration.GetSection("AiursoftAuthentication"));
        services.AddAiursoftObserver(Configuration.GetSection("AiursoftObserver"));
        services.AddAiursoftSdk();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiursoftHandler(env.IsDevelopment());
        app.UseWebSockets();
        app.UseAiursoftAppRouters();
    }
}