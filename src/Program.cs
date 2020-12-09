using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;
using static Aiursoft.WebTools.Extends;

namespace Tracer
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            App<Startup>(args).Run();
        }
    }
}
