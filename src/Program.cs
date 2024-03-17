using Aiursoft.WebTools.Abstractions;
using Aiursoft.WebTools.OfficialPlugins;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Tracer;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = await AppAsync<Startup>(args, plugins: new List<IWebAppPlugin>()
        {
            new DockerPlugin()
        });
        await app.RunAsync();
    }
}