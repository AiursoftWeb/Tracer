using Aiursoft.Tracer.Services.FileStorage;
using Microsoft.Extensions.Caching.Memory;

namespace Aiursoft.Tracer.Tests;

[TestClass]
public class FileAccessSecurityTest
{
    private StorageService _storageService = null!;
    private string _tempPath = null!;

    [TestInitialize]
    public void Initialize()
    {
        _tempPath = Path.Combine(Path.GetTempPath(), "AiursoftTemplateTest_" + Guid.NewGuid());
        Directory.CreateDirectory(_tempPath);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Storage:Path", _tempPath }
            })
            .Build();

        var rootProvider = new StorageRootPathProvider(config);
        var foldersProvider = new FeatureFoldersProvider(rootProvider);
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var fileLockProvider = new FileLockProvider(memoryCache);

        _storageService = new StorageService(foldersProvider, fileLockProvider);
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_tempPath))
        {
            Directory.Delete(_tempPath, true);
        }
    }

    [TestMethod]
    public void TestGetFilePhysicalPath_NormalAccess()
    {
        var relativePath = "test.txt";
        var physicalPath = _storageService.GetFilePhysicalPath(relativePath);

        StringAssert.StartsWith(physicalPath, _tempPath);
        StringAssert.EndsWith(physicalPath, relativePath);
    }

    [TestMethod]
    [DataRow("../secret.txt")]
    [DataRow("../../etc/passwd")]
    [DataRow("/etc/passwd")]
    public void TestGetFilePhysicalPath_PathTraversal(string maliciousPath)
    {
        try
        {
            _storageService.GetFilePhysicalPath(maliciousPath);
            Assert.Fail("Expected ArgumentException was not thrown.");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public async Task TestSave_NormalAccess()
    {
        var content = "Hello World";
        var fileName = "test_upload.txt";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        var formFile = new FormFile(ms, 0, ms.Length, "file", fileName);

        var savedPath = await _storageService.Save("uploads/" + fileName, formFile);

        StringAssert.Contains(savedPath, "uploads");
        StringAssert.Contains(savedPath, fileName);
    }

    [TestMethod]
    [DataRow("../malicious.txt")]
    [DataRow("../../malicious.txt")]
    [DataRow("/absolute/path/malicious.txt")]
    public async Task TestSave_PathTraversal(string maliciousPath)
    {
        var ms = new MemoryStream();
        var formFile = new FormFile(ms, 0, 0, "file", "dummy.txt");

        try
        {
            await _storageService.Save(maliciousPath, formFile);
            Assert.Fail("Expected ArgumentException was not thrown.");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }
}
