using Aiursoft.DbTools;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Tracer.Entities;

public abstract class TemplateDbContext(DbContextOptions options) : IdentityDbContext<User>(options), ICanMigrate
{
    public virtual  Task MigrateAsync(CancellationToken cancellationToken) =>
        Database.MigrateAsync(cancellationToken);

    public virtual  Task<bool> CanConnectAsync() =>
        Database.CanConnectAsync();
}
