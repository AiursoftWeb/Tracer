using Aiursoft.Canon.BackgroundJobs;
using Aiursoft.Tracer.Entities;
using Aiursoft.Tracer.Services.FileStorage;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Tracer.Services.BackgroundJobs;

public class OrphanAvatarCleanupJob(
    TracerDbContext db,
    FeatureFoldersProvider folders,
    ILogger<OrphanAvatarCleanupJob> logger) : IBackgroundJob
{
    public string Name => "Orphan Avatar Cleanup";
    public string Description => "Scans the avatar storage directory and deletes image files that are no longer referenced by any user account, freeing disk space.";
    public async Task ExecuteAsync()
    {
        logger.LogInformation("Starting orphan avatar cleanup job...");
        // 1. Collect all avatar paths currently referenced by users
        var referencedPaths = await db.Users.Select(u => u.AvatarRelativePath).ToHashSetAsync();
        referencedPaths.Add(User.DefaultAvatarPath);
        // 2. Scan workspace for avatar/ files
        var workspace = folders.GetWorkspaceFolder();
        var avatarDir = Path.Combine(workspace, "avatar");
        if (!Directory.Exists(avatarDir)) return;
        var allAvatarFiles = Directory.EnumerateFiles(avatarDir, "*", SearchOption.AllDirectories).ToList();
        // 3. Delete orphans
        foreach (var physicalPath in allAvatarFiles)
        {
            var relativePath = Path.GetRelativePath(workspace, physicalPath).Replace('\\', '/');
            if (!referencedPaths.Contains(relativePath)) File.Delete(physicalPath);
        }
        logger.LogInformation("Orphan avatar cleanup job completed.");
    }
}
