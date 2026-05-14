using System.Reflection;
using Aiursoft.Tracer.Authorization;
using Aiursoft.Tracer.Entities;
using Aiursoft.Tracer.Services;
using Aiursoft.UiStack.Navigation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Aiursoft.Tracer.Models.SystemViewModels;
using Aiursoft.WebTools.Attributes;

namespace Aiursoft.Tracer.Controllers;

/// <summary>
/// This controller is used to handle system related actions like shutdown.
/// </summary>
[Authorize]
[LimitPerMin]
public class SystemController(ILogger<SystemController> logger, TracerDbContext dbContext) : Controller
{
    [Authorize(Policy = AppPermissionNames.CanViewSystemContext)]
    [RenderInNavBar(
        NavGroupName = "Administration",
        NavGroupOrder = 9999,
        CascadedLinksGroupName = "System",
        CascadedLinksIcon = "cog",
        CascadedLinksOrder = 9999,
        LinkText = "Info",
        LinkOrder = 1)]
    public async Task<IActionResult> Index()
    {
        var tableCounts = await GetTableCountsAsync();
        var (applied, defined, pending) = await GetMigrationInfoAsync();
        return this.StackView(new IndexViewModel
        {
            TableCounts = tableCounts,
            AppliedMigrations = applied,
            TotalDefinedMigrations = defined,
            PendingMigrations = pending,
        });
    }

    private async Task<Dictionary<string, long>> GetTableCountsAsync()
    {
        var tableCounts = new Dictionary<string, long>();
        var visitedNames = new HashSet<string>();
        var longCountAsync = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == nameof(EntityFrameworkQueryableExtensions.LongCountAsync)
                        && m.GetParameters().Length == 2);

        for (var type = dbContext.GetType(); type != null && type != typeof(object); type = type.BaseType)
        {
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (!visitedNames.Add(prop.Name)) continue;
                if (!prop.PropertyType.IsGenericType) continue;
                if (prop.PropertyType.GetGenericTypeDefinition() != typeof(DbSet<>)) continue;

                var entityType = prop.PropertyType.GetGenericArguments()[0];
                if (dbContext.Model.FindEntityType(entityType) == null) continue;

                var dbSet = prop.GetValue(dbContext);
                if (dbSet == null) continue;

                try
                {
                    var count = await (Task<long>)longCountAsync
                        .MakeGenericMethod(entityType)
                        .Invoke(null, [dbSet, CancellationToken.None])!;
                    tableCounts[prop.Name] = count;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to count rows for DbSet property '{PropertyName}'", prop.Name);
                }
            }
        }

        return tableCounts;
    }

    private async Task<(List<MigrationEntry> applied, int defined, List<string> pending)> GetMigrationInfoAsync()
    {
        try
        {
            var appliedIds = (await dbContext.Database.GetAppliedMigrationsAsync())
                .Select(id => new MigrationEntry { Id = id })
                .ToList();
            var definedCount = dbContext.Database.GetMigrations().Count();
            var pendingList = (await dbContext.Database.GetPendingMigrationsAsync()).ToList();
            return (appliedIds, definedCount, pendingList);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to retrieve migration information");
            return ([], 0, []);
        }
    }

    [HttpPost]
    [Authorize(Policy = AppPermissionNames.CanRebootThisApp)] // Use the specific permission
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult Shutdown([FromServices] IHostApplicationLifetime appLifetime)
    {
        logger.LogWarning("Application shutdown was requested by user: '{UserName}'", User.Identity?.Name);
        appLifetime.StopApplication();
        return Accepted();
    }
}