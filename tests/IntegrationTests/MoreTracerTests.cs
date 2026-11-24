using Aiursoft.Tracer.Configuration;
using Aiursoft.Tracer.Services;
using Aiursoft.Tracer.Services.FileStorage;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Aiursoft.Tracer.Tests.IntegrationTests;

[TestClass]
public class MoreTracerTests : TestBase
{
    [TestMethod]
    public async Task TestProgramExtends()
    {
        await Server!.CopyAvatarFileAsync();
        await Server!.CopyAvatarFileAsync();
    }

    [TestMethod]
    public void TestPathResolver()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "PathResolverTests_" + Guid.NewGuid());
        Directory.CreateDirectory(testDir);
        try
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Storage:Path"] = testDir
                })
                .Build();

            var resolver = new PathResolver(config);

            var workspaceFile = Path.Combine(testDir, "Workspace", "sub", "test.png");
            Directory.CreateDirectory(Path.GetDirectoryName(workspaceFile)!);
            File.WriteAllText(workspaceFile, "fake content");

            var cleared = resolver.GetClearedExifAbsolutePath(workspaceFile);
            Assert.IsTrue(cleared.Contains("ClearExif"));
            Assert.IsTrue(cleared.EndsWith("test.png"));

            var compressedBoth = resolver.GetCompressedAbsolutePath(workspaceFile, 100, 200);
            Assert.IsTrue(compressedBoth.Contains("Compressed"));
            Assert.IsTrue(compressedBoth.Contains("_w100_h200.png"));

            var compressedW = resolver.GetCompressedAbsolutePath(workspaceFile, 100, 0);
            Assert.IsTrue(compressedW.Contains("_w100.png"));

            var compressedH = resolver.GetCompressedAbsolutePath(workspaceFile, 0, 200);
            Assert.IsTrue(compressedH.Contains("_h200.png"));

            var compressedNone = resolver.GetCompressedAbsolutePath(workspaceFile, 0, 0);
            Assert.IsTrue(compressedNone.EndsWith("test.png"));
        }
        finally
        {
            if (Directory.Exists(testDir))
            {
                Directory.Delete(testDir, true);
            }
        }
    }

    [TestMethod]
    public void TestPathResolverMissingConfigThrows()
    {
        var config = new ConfigurationBuilder().Build();
        try
        {
            _ = new PathResolver(config);
            Assert.Fail("Should have thrown InvalidDataException");
        }
        catch (InvalidDataException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void TestZeroStreamOperations()
    {
        var stream = new ZeroStream(50);
        Assert.IsTrue(stream.CanRead);
        Assert.IsFalse(stream.CanSeek);
        Assert.IsFalse(stream.CanWrite);
        Assert.AreEqual(50, stream.Length);

        var buf = new byte[30];
        var read = stream.Read(buf, 0, 30);
        Assert.AreEqual(30, read);
        Assert.IsTrue(buf.All(b => b == 0));

        var buf2 = new byte[30];
        var read2 = stream.Read(buf2, 0, 30);
        Assert.AreEqual(20, read2);

        var read3 = stream.Read(buf2, 0, 10);
        Assert.AreEqual(0, read3);

        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => Task.FromResult(new ZeroStream(-1)));
        Assert.ThrowsAsync<ArgumentNullException>(() => Task.FromResult(stream.Read(null!, 0, 10)));
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => Task.FromResult(stream.Read(buf, -1, 10)));
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => Task.FromResult(stream.Read(buf, 0, -1)));
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => Task.FromResult(stream.Read(buf, 20, 20)));

        Assert.ThrowsAsync<NotSupportedException>(() => Task.FromResult(stream.Position));
        Assert.ThrowsAsync<NotSupportedException>(() =>
        {
            stream.Position = 0;
            return Task.CompletedTask;
        });
        Assert.ThrowsAsync<NotSupportedException>(() =>
        {
            stream.Flush();
            return Task.CompletedTask;
        });
        Assert.ThrowsAsync<NotSupportedException>(() => Task.FromResult(stream.Seek(0, SeekOrigin.Begin)));
        Assert.ThrowsAsync<NotSupportedException>(() =>
        {
            stream.SetLength(10);
            return Task.CompletedTask;
        });
        Assert.ThrowsAsync<NotSupportedException>(() =>
        {
            stream.Write([1, 2], 0, 2);
            return Task.CompletedTask;
        });
    }

    [TestMethod]
    public async Task TestHomeControllerExtra()
    {
        var selfhostResp = await Http.GetAsync("/Home/Selfhost");
        selfhostResp.EnsureSuccessStatusCode();

        using var req = new HttpRequestMessage(HttpMethod.Get, "/download.dat");
        req.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(0, 100);
        var dlResp = await Http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
        Assert.IsTrue(dlResp.IsSuccessStatusCode);

        var upResp = await Http.PostAsync("/upload", new StringContent("some test content"));
        Assert.IsTrue(upResp.IsSuccessStatusCode);
    }

    [TestMethod]
    public async Task TestViewComponents()
    {
        using var scope = Server!.Services.CreateScope();
        var globalSettings = scope.ServiceProvider.GetRequiredService<GlobalSettingsService>();
        var storage = scope.ServiceProvider.GetRequiredService<StorageService>();

        var footer = new Views.Shared.Components.MarketingFooter.MarketingFooter(globalSettings, storage)
        {
            ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext { HttpContext = new DefaultHttpContext() }
            }
        };
        var footRes = await footer.InvokeAsync();
        Assert.IsNotNull(footRes);
    }

    [TestMethod]
    public async Task TestGlobalSettingsService()
    {
        var service = GetService<GlobalSettingsService>();
        await service.UpdateSettingAsync(SettingsMap.AllowUserAdjustNickname, "False");
        var val = await service.GetBoolSettingAsync(SettingsMap.AllowUserAdjustNickname);
        Assert.IsFalse(val);

        await service.UpdateSettingAsync(SettingsMap.AllowUserAdjustNickname, "True");
        var val2 = await service.GetBoolSettingAsync(SettingsMap.AllowUserAdjustNickname);
        Assert.IsTrue(val2);
    }
}
