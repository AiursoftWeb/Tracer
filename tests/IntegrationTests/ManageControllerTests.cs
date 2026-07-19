using Aiursoft.Tracer.Services;

using Aiursoft.Tracer.Entities;
using System.Net;
using Microsoft.AspNetCore.Identity;
namespace Aiursoft.Tracer.Tests.IntegrationTests;

[TestClass]
public class ManageControllerTests : TestBase
{
    [TestMethod]
    public async Task TestManageWorkflow()
    {
        await LoginAsAdmin();

        // Ensure AllowUserAdjustNickname is true
        using (var scope = Server!.Services.CreateScope())
        {
            var settingsService = scope.ServiceProvider.GetRequiredService<GlobalSettingsService>();
            await settingsService.UpdateSettingAsync(Configuration.SettingsMap.AllowUserAdjustNickname, "True");
        }

        // 1. Index
        var indexResponse = await Http.GetAsync("/Manage/Index");
        indexResponse.EnsureSuccessStatusCode();

        // 2. ChangePassword (GET)
        var changePasswordPage = await Http.GetAsync("/Manage/ChangePassword");
        changePasswordPage.EnsureSuccessStatusCode();

        // 3. ChangeProfile (GET)
        var changeProfilePage = await Http.GetAsync("/Manage/ChangeProfile");
        changeProfilePage.EnsureSuccessStatusCode();

        // 4. ChangeAvatar (GET)
        var changeAvatarPage = await Http.GetAsync("/Manage/ChangeAvatar");
        changeAvatarPage.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public async Task TestDeleteAccount_Authenticated_DeletesUserAndSignsOut()
    {
        // Arrange: register and login as a new user
        var (email, _) = await RegisterAndLoginAsync();

        // Act: confirmation page loads
        var deletePage = await Http.GetAsync("/Manage/DeleteAccount");
        deletePage.EnsureSuccessStatusCode();

        // Act: confirm deletion
        var deleteResponse = await PostForm("/Manage/DeleteAccountPost", new(), tokenUrl: "/Manage/DeleteAccount");
        AssertRedirect(deleteResponse, "/");

        // Assert: signed out — accessing a protected page should redirect to login
        var managePage = await Http.GetAsync("/Manage/Index");
        Assert.AreEqual(HttpStatusCode.Found, managePage.StatusCode);

        // Assert: user no longer exists in the database
        using var scope = Server!.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        Assert.IsNull(await userManager.FindByEmailAsync(email));
    }

    [TestMethod]
    public async Task TestDeleteAccount_Unauthenticated_RedirectsToLogin()
    {
        // No login — direct access should redirect
        var deletePage = await Http.GetAsync("/Manage/DeleteAccount");
        Assert.AreEqual(HttpStatusCode.Found, deletePage.StatusCode);
    }
}
