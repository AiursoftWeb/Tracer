using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Tracer;

public class Program
{
    public static async Task Main(string[] args)
    {
        await App<Startup>(args).RunAsync();
    }
}