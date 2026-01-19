using System.Net;
using Aiursoft.Tracer.Entities;

namespace Aiursoft.Tracer.Tests.IntegrationTests;

[TestClass]
public class UsersControllerTests : TestBase
{
    [TestMethod]
    public async Task TestUsersWorkflow()
    {
        await LoginAsAdmin();

        // 1. Index
        var indexResponse = await Http.GetAsync("/Users/Index");
        indexResponse.EnsureSuccessStatusCode();
        var indexHtml = await indexResponse.Content.ReadAsStringAsync();
        Assert.Contains("admin", indexHtml);

        // 2. Create
        var userName = "testuser-" + Guid.NewGuid();
        var email = userName + "@aiursoft.com";
        var createResponse = await PostForm("/Users/Create", new Dictionary<string, string>
        {
            { "UserName", userName },
            { "DisplayName", "Test User" },
            { "Email", email },
            { "Password", "TestPassword123" }
        });
        AssertRedirect(createResponse, "/Users/Details/", exact: false);
        var userId = createResponse.Headers.Location?.OriginalString.Split("/").Last();
        Assert.IsNotNull(userId);

        // 3. Details
        var detailsResponse = await Http.GetAsync($"/Users/Details/{userId}");
        detailsResponse.EnsureSuccessStatusCode();
        var detailsHtml = await detailsResponse.Content.ReadAsStringAsync();
        Assert.Contains(userName, detailsHtml);

        // 4. Edit (GET)
        var editPageResponse = await Http.GetAsync($"/Users/Edit/{userId}");
        editPageResponse.EnsureSuccessStatusCode();

        // 5. Edit (POST)
        var newDisplayName = "Updated Test User";
        var editResponse = await PostForm($"/Users/Edit/{userId}", new Dictionary<string, string>
        {
            { "Id", userId },
            { "UserName", userName },
            { "Email", email },
            { "DisplayName", newDisplayName },
            { "AvatarUrl", User.DefaultAvatarPath }
        });
        AssertRedirect(editResponse, "/Users/Details/", exact: false);

        // 6. ManageRoles (POST)
        var manageRolesResponse = await PostForm($"/Users/ManageRoles/{userId}", new Dictionary<string, string>
        {
            { "id", userId },
            { "AllRoles[0].RoleName", "Administrators" },
            { "AllRoles[0].IsSelected", "true" }
        }, tokenUrl: $"/Users/Edit/{userId}");
        AssertRedirect(manageRolesResponse, "/Users/Details/", exact: false);

        // 7. Delete (GET)
        var deletePageResponse = await Http.GetAsync($"/Users/Delete/{userId}");
        deletePageResponse.EnsureSuccessStatusCode();

        // 8. Delete (POST)
        var deleteResponse = await PostForm($"/Users/Delete/{userId}", new Dictionary<string, string>
        {
            { "id", userId }
        });
        AssertRedirect(deleteResponse, "/Users");
    }

    [TestMethod]
    public async Task TestDetailsNotFound()
    {
        await LoginAsAdmin();
        var response = await Http.GetAsync("/Users/Details/invalid-id");
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task TestEditNotFound()
    {
        await LoginAsAdmin();
        var response = await Http.GetAsync("/Users/Edit/invalid-id");
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task TestDeleteNotFound()
    {
        await LoginAsAdmin();
        var response = await Http.GetAsync("/Users/Delete/invalid-id");
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}
