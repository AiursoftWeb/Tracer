﻿using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Tracer;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = await AppAsync<Startup>(args);
        await app.RunAsync();
    }
}