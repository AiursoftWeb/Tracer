using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;

namespace Tracer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            App<Startup>(args).Run();
        }
    }
}
