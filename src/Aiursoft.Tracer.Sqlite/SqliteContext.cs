using Aiursoft.Tracer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Tracer.Sqlite;

public class SqliteContext(DbContextOptions<SqliteContext> options) : TemplateDbContext(options)
{
    public override Task<bool> CanConnectAsync()
    {
        return Task.FromResult(true);
    }
}
