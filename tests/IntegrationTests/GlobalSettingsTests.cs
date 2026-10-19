using Microsoft.EntityFrameworkCore;
using Aiursoft.Tracer.Entities;
using System.Net;
using Aiursoft.Tracer.Configuration;
using Aiursoft.Tracer.Services;

namespace Aiursoft.Tracer.Tests.IntegrationTests;

[TestClass]
public class GlobalSettingsTests : TestBase
{
    [TestMethod]
    public async Task SecretSettingsPreserveLegacyValuesAndHideSavedSecrets()
    {
        const string secretKey = "IntegrationTest_Secret";
        const string textKey = "IntegrationTest_LegacyText";
        const string legacySecret = "legacy-test-secret-value";
        const string legacyText = "protected:v1:literal-text-value";
        var secretDefinition = new Models.GlobalSettingDefinition
        {
            Key = secretKey, Name = "Test secret", Description = "Test only", DefaultValue = "",
            Type = Models.SettingType.Secret
        };
        var textDefinition = new Models.GlobalSettingDefinition
        {
            Key = textKey, Name = "Test text", Description = "Test only", DefaultValue = "",
            Type = Models.SettingType.Text
        };
        SettingsMap.Definitions.Add(secretDefinition);
        SettingsMap.Definitions.Add(textDefinition);
        using var scope = Server!.Services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var cache = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
        try
        {
            var settings = scope.ServiceProvider.GetRequiredService<GlobalSettingsService>();
            var db = scope.ServiceProvider.GetRequiredService<TracerDbContext>();
            db.GlobalSettings.Add(new GlobalSetting { Key = secretKey, Value = legacySecret });
            db.GlobalSettings.Add(new GlobalSetting { Key = textKey, Value = legacyText });
            await db.SaveChangesAsync();
            Assert.AreEqual(legacySecret, await settings.GetSettingValueAsync(secretKey));
            Assert.AreEqual(legacyText, await settings.GetSettingValueAsync(textKey));
            await settings.SeedSettingsAsync();
            Assert.AreEqual(legacySecret, (await db.GlobalSettings.AsNoTracking().SingleAsync(s => s.Key == secretKey)).Value);
            Assert.AreEqual(legacyText, (await db.GlobalSettings.AsNoTracking().SingleAsync(s => s.Key == textKey)).Value);
            await settings.UpdateSettingAsync(textKey, legacyText);
            Assert.AreEqual(legacyText, (await db.GlobalSettings.AsNoTracking().SingleAsync(s => s.Key == textKey)).Value);
            await LoginAsAdmin();
            Assert.DoesNotContain(legacySecret, await Http.GetStringAsync("/GlobalSettings/Index"));
            await settings.UpdateSettingAsync(secretKey, "replacement-test-secret");
            var storedValue = (await db.GlobalSettings.AsNoTracking().SingleAsync(s => s.Key == secretKey)).Value!;
            Assert.StartsWith("protected:v1:", storedValue);
            Assert.DoesNotContain("replacement-test-secret", storedValue);
            Assert.AreEqual("replacement-test-secret", await settings.GetSettingValueAsync(secretKey));
            var html = await Http.GetStringAsync("/GlobalSettings/Index");
            Assert.Contains(secretKey, html);
            Assert.DoesNotContain("replacement-test-secret", html);
            Assert.DoesNotContain(storedValue, html);
            configuration[$"GlobalSettings:{secretKey}"] = "override-test-secret";
            cache.Remove($"global-setting-{secretKey}");
            Assert.AreEqual("override-test-secret", await settings.GetSettingValueAsync(secretKey));
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => settings.UpdateSettingAsync(secretKey, "replacement"));
            Assert.DoesNotContain("override-test-secret", await Http.GetStringAsync("/GlobalSettings/Index"));
            configuration[$"GlobalSettings:{secretKey}"] = null;
            cache.Remove($"global-setting-{secretKey}");
            await settings.UpdateSettingAsync(secretKey, "");
            Assert.AreEqual("", await settings.GetSettingValueAsync(secretKey));
        }
        finally
        {
            configuration[$"GlobalSettings:{secretKey}"] = null;
            cache.Remove($"global-setting-{secretKey}");
            cache.Remove($"global-setting-{textKey}");
            SettingsMap.Definitions.Remove(secretDefinition);
            SettingsMap.Definitions.Remove(textDefinition);
        }
    }

    [TestMethod]
    public async Task TestAllowUserAdjustNicknameSetting()
    {
        // 1. Login as admin
        await LoginAsAdmin();

        // 2. Disable Allow_User_Adjust_Nickname
        using (var scope = Server!.Services.CreateScope())
        {
            var settingsService = scope.ServiceProvider.GetRequiredService<GlobalSettingsService>();
            await settingsService.UpdateSettingAsync(SettingsMap.AllowUserAdjustNickname, "False");
        }

        // 3. Verify that the "Change your profile" link is NOT visible on Manage/Index
        var manageIndexResponse = await Http.GetAsync("/Manage/Index");
        var manageIndexHtml = await manageIndexResponse.Content.ReadAsStringAsync();
        Assert.DoesNotContain("Change your profile", manageIndexHtml);

        // 4. Verify that accessing /Manage/ChangeProfile directly returns BadRequest
        var changeProfileResponse = await Http.GetAsync("/Manage/ChangeProfile");
        Assert.AreEqual(HttpStatusCode.BadRequest, changeProfileResponse.StatusCode);

        // 5. Enable Allow_User_Adjust_Nickname
        using (var scope = Server!.Services.CreateScope())
        {
            var settingsService = scope.ServiceProvider.GetRequiredService<GlobalSettingsService>();
            await settingsService.UpdateSettingAsync(SettingsMap.AllowUserAdjustNickname, "True");
        }

        // 6. Verify that the "Change your profile" link IS visible on Manage/Index
        manageIndexResponse = await Http.GetAsync("/Manage/Index");
        manageIndexHtml = await manageIndexResponse.Content.ReadAsStringAsync();
        Assert.Contains("Change your profile", manageIndexHtml);

        // 7. Verify that accessing /Manage/ChangeProfile directly returns OK
        changeProfileResponse = await Http.GetAsync("/Manage/ChangeProfile");
        Assert.AreEqual(HttpStatusCode.OK, changeProfileResponse.StatusCode);
    }

    [TestMethod]
    public async Task TestAdminManageSettings()
    {
        // 1. Login as admin
        await LoginAsAdmin();

        // 2. Access Global Settings Index
        var settingsResponse = await Http.GetAsync("/GlobalSettings/Index");
        settingsResponse.EnsureSuccessStatusCode();
        var settingsHtml = await settingsResponse.Content.ReadAsStringAsync();
        Assert.Contains("Global Settings", settingsHtml);
        Assert.Contains(SettingsMap.AllowUserAdjustNickname, settingsHtml);

        // 3. Change setting via UI
        var editResponse = await PostForm("/GlobalSettings/Edit", new Dictionary<string, string>
        {
            { "Key", SettingsMap.AllowUserAdjustNickname },
            { "Value", "False" }
        }, tokenUrl: "/GlobalSettings/Index");
        Assert.AreEqual(HttpStatusCode.Found, editResponse.StatusCode);

        // 4. Verify setting changed in DB
        using (var scope = Server!.Services.CreateScope())
        {
            var settingsService = scope.ServiceProvider.GetRequiredService<GlobalSettingsService>();
            var value = await settingsService.GetBoolSettingAsync(SettingsMap.AllowUserAdjustNickname);
            Assert.IsFalse(value);
        }
    }

    [TestMethod]
    public async Task TestGlobalSettingsServiceValidation()
    {
        using var scope = Server!.Services.CreateScope();
        var settingsService = scope.ServiceProvider.GetRequiredService<GlobalSettingsService>();

        // Test non-defined setting
        try
        {
            await settingsService.UpdateSettingAsync("InvalidKey", "Value");
            Assert.Fail("Should have thrown InvalidOperationException");
        }
        catch (InvalidOperationException) { }

        // Test Bool validation
        try
        {
            await settingsService.UpdateSettingAsync(SettingsMap.AllowUserAdjustNickname, "NotABool");
            Assert.Fail("Should have thrown InvalidOperationException");
        }
        catch (InvalidOperationException) { }
    }
}
