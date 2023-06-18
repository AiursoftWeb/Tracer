using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;

namespace Tracer;

public class Program
{
    public static async Task Main(string[] args)
    {
        await App<Startup>(args).RunAsync();
    }
}