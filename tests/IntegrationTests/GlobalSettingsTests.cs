using System.Net;
using Aiursoft.Tracer.Configuration;
using Aiursoft.Tracer.Services;

namespace Aiursoft.Tracer.Tests.IntegrationTests;

[TestClass]
public class GlobalSettingsTests : TestBase
{
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
