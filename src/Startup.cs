using Aiursoft.Archon.SDK;
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Observer.SDK;
using Aiursoft.SDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Tracer
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AppsContainer.CurrentAppId = configuration["TracerAppId"];
            AppsContainer.CurrentAppSecret = configuration["TracerAppSecret"];
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAiurMvc();
            services.AddArchonServer(Configuration.GetConnectionString("ArchonConnection"));
            services.AddObserverServer(Configuration.GetConnectionString("ObserverConnection"));
            services.AddAiursoftSDK();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAiurUserHandler(env.IsDevelopment());
            app.UseWebSockets();
            app.UseAiursoftDefault();
        }
    }
}
