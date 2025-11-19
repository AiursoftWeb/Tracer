using Aiursoft.Scanner.Abstractions;

namespace Aiursoft.Tracer.Services.FileStorage;

/// <summary>
/// Represents a service for storing and managing files.
/// </summary>
public class StorageService(IConfiguration configuration) : ISingletonDependency
{
    public readonly string StorageRootFolder = configuration["Storage:Path"]!;

    // Async lock.
    private readonly SemaphoreSlim _lock = new(1, 1);

    /// <summary>
    /// Saves a file to the storage.
    /// </summary>
    /// <param name="savePath">The path where the file will be saved. The 'savePath' is the path that the user wants to save. Not related to actual disk path.</param>
    /// <param name="file">The file to be saved.</param>
    /// <returns>The actual path where the file is saved relative to the workspace folder.</returns>
    public async Task<string> Save(string savePath, IFormFile file)
    {
        var finalFilePath = Path.Combine(StorageRootFolder, "Workspace", savePath);
        var finalFolder = Path.GetDirectoryName(finalFilePath);

        // Create the folder if it does not exist.
        if (!Directory.Exists(finalFolder))
        {
            Directory.CreateDirectory(finalFolder!);
        }

        // The problem is: What if the file already exists?
        await _lock.WaitAsync();
        try
        {
            var expectedFileName = Path.GetFileName(finalFilePath);
            while (File.Exists(finalFilePath))
            {
                expectedFileName = "_" + expectedFileName;
                finalFilePath = Path.Combine(finalFolder!, expectedFileName);
            }

            // This is to avoid the case that the file already exists.
            // However, we can't copy the stream to the new file. Because this is running in a lock and we need to release the lock ASAP.
            // So we create a new file and close it to ensure the file is valid and can be copied to.
            File.Create(finalFilePath).Close();
        }
        finally
        {
            _lock.Release();
        }

        await using var fileStream = new FileStream(finalFilePath, FileMode.Create);
        await file.CopyToAsync(fileStream);
        fileStream.Close();

        return Path.GetRelativePath(StorageRootFolder, finalFilePath);
    }

    /// <summary>
    /// Retrieves the physical file path for a given file name within the storage workspace folder.
    /// </summary>
    /// <param name="relativePath">The name of the file for which the physical path is required.</param>
    /// <returns>The full physical path of the file within the workspace folder.</returns>
    public string GetFilePhysicalPath(string relativePath)
    {
        return Path.Combine(StorageRootFolder, relativePath);
    }

    /// <summary>
    /// Converts a relative path to a URI-compatible path.
    /// </summary>
    /// <param name="relativePath">The relative file path to be converted.</param>
    /// <returns>A URI-compatible path derived from the relative path.</returns>
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
