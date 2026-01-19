using System.Diagnostics.CodeAnalysis;
using Aiursoft.DbTools;
using Aiursoft.Tracer.Entities;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Tracer;

[ExcludeFromCodeCoverage]
public abstract class Program
{
    public static async Task Main(string[] args)
    {
        var app = await AppAsync<Startup>(args);
        await app.UpdateDbAsync<TracerDbContext>();
        await app.SeedAsync();
        await app.CopyAvatarFileAsync();
        await app.RunAsync();
    }
}
