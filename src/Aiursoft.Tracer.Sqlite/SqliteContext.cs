using System.Diagnostics.CodeAnalysis;
using Aiursoft.Tracer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Tracer.Sqlite;

[ExcludeFromCodeCoverage]

public class SqliteContext(DbContextOptions<SqliteContext> options) : TracerDbContext(options)
{
    public override Task<bool> CanConnectAsync()
    {
        return Task.FromResult(true);
    }
}
