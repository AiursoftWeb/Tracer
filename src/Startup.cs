using Aiursoft.Directory.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.SDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Tracer;

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
        services.AddAiursoftAuthentication(Configuration.GetSection("AiursoftAuthentication"));
        services.AddAiursoftObserver(Configuration.GetSection("AiursoftObserver"));
        services.AddAiursoftSDK();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiurUserHandler(env.IsDevelopment());
        app.UseWebSockets();
        app.UseAiursoftDefault();
    }
}