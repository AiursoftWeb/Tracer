using Aiursoft.Scanner.Abstractions;

namespace Aiursoft.Tracer.Services.FileStorage;

/// <summary>
/// Represents a service for storing and managing files. (Level 3: Business Gateway)
/// </summary>
public class StorageService(
    FeatureFoldersProvider folders,
    FileLockProvider fileLockProvider) : ITransientDependency
{
    /// <summary>
    /// Saves a file to the storage.
    /// </summary>
    /// <param name="logicalPath">The logical path (relative to Workspace) where the file will be saved.</param>
    /// <param name="file">The file to be saved.</param>
    /// <returns>The actual logical path where the file is saved (may differ if renamed).</returns>
    public async Task<string> Save(string logicalPath, IFormFile file)
    {
        // 1. Get Workspace root
        var workspaceRoot = folders.GetWorkspaceFolder();
        
        // 2. Resolve physical path
        var physicalPath = Path.GetFullPath(Path.Combine(workspaceRoot, logicalPath));

        // 3. Security check: Ensure path is within Workspace
        if (!physicalPath.StartsWith(workspaceRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Path traversal attempt detected!");
        }

        // 4. Create directory if needed
        var directory = Path.GetDirectoryName(physicalPath);
        if (!Directory.Exists(directory))
        {
             Directory.CreateDirectory(directory!);
        }

        // 5. Handle collisions (Renaming)
        // Lock on the directory to prevent race conditions during renaming
        var lockObj = fileLockProvider.GetLock(directory!); 
        await lockObj.WaitAsync();
        try
        {
            var expectedFileName = Path.GetFileName(physicalPath);
            while (File.Exists(physicalPath))
            {
                expectedFileName = "_" + expectedFileName;
                physicalPath = Path.Combine(directory!, expectedFileName);
            }

            // Create placeholder to reserve name
            File.Create(physicalPath).Close();
        }
        finally
        {
            lockObj.Release();
        }

        // 6. Write file content
        await using var fileStream = new FileStream(physicalPath, FileMode.Create);
        await file.CopyToAsync(fileStream);
        
        // 7. Return logical path (relative to Workspace)
        return Path.GetRelativePath(workspaceRoot, physicalPath).Replace("\\", "/");
    }

    /// <summary>
    /// Retrieves the physical file path for a given logical path.
    /// Defaults to Workspace.
    /// </summary>
    public string GetFilePhysicalPath(string logicalPath)
    {
        var workspaceRoot = folders.GetWorkspaceFolder();
        var physicalPath = Path.GetFullPath(Path.Combine(workspaceRoot, logicalPath));

        if (!physicalPath.StartsWith(workspaceRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Restricted path access!");
        }
        return physicalPath;
    }

    /// <summary>
    /// Converts a logical path to a URI-compatible path.
    /// </summary>
    private string RelativePathToUriPath(string relativePath)
    {
        var urlPath = Uri.EscapeDataString(relativePath)
            .Replace("%5C", "/")
            .Replace("%5c", "/")
            .Replace("%2F", "/")
            .Replace("%2f", "/")
            .TrimStart('/');
        return urlPath;
    }

    public string RelativePathToInternetUrl(string relativePath, HttpContext context)
    {
        return $"{context.Request.Scheme}://{context.Request.Host}/download/{RelativePathToUriPath(relativePath)}";
    }

    public string RelativePathToInternetUrl(string relativePath)
    {
        return $"/download/{RelativePathToUriPath(relativePath)}";
    }
}
