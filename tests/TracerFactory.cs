using Aiursoft.Archon.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.Scanner;
using Aiursoft.SDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracer.Tests
{
    public class TracerFactory
    {
        public static IHost BuildTracer(int port)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls($"http://localhost:{port}");
                    webBuilder.UseStartup<TestStartUp>();
                })
                .Build();
        }
    }

    public class TestStartUp : Startup
    {
        public TestStartUp(IConfiguration configuration) : base(configuration) { }
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddLibraryDependencies();
        }
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAiurUserHandler(true);
            app.UseWebSockets();
            app.UseAiursoftDefault();
        }
    }
}
