using System.Net;
using System.Text.RegularExpressions;
using Aiursoft.AiurObserver.DefaultConsumers;
using Aiursoft.AiurObserver.WebSocket;
using Aiursoft.CSTools.Tools;
using Aiursoft.DbTools;
using Aiursoft.Tracer.Entities;
using static Aiursoft.WebTools.Extends;

[assembly:DoNotParallelize]

namespace Aiursoft.Tracer.Tests.IntegrationTests;

[TestClass]
public class BasicTests
{
    private readonly int _port;
    private readonly HttpClient _http;
    private IHost? _server;

    public BasicTests()
    {
        var cookieContainer = new CookieContainer();
        var handler = new HttpClientHandler
        {
            CookieContainer = cookieContainer,
            AllowAutoRedirect = false
        };
        _port = Network.GetAvailablePort();
        _http = new HttpClient(handler)
        {
            BaseAddress = new Uri($"http://localhost:{_port}")
        };
    }

    [TestInitialize]
    public async Task CreateServer()
    {
        _server = await AppAsync<Startup>([], port: _port);
        await _server.UpdateDbAsync<TemplateDbContext>();
        await _server.SeedAsync();
        await _server.StartAsync();
    }

    [TestCleanup]
    public async Task CleanServer()
    {
        if (_server == null) return;
        await _server.StopAsync();
        _server.Dispose();
    }

    [TestMethod]
    [DataRow("/")]
    [DataRow("/hOmE?aaaaaa=bbbbbb")]
    [DataRow("/hOmE/InDeX")]
    public async Task GetHome(string url)
    {
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public async Task GetDownload()
    {
        var url = "/download.dat";
        var response = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode(); // Status Code 200-299
    }

    [TestMethod]
    public async Task PostUpload()
    {
        var url = "/upload";
        var content = new ByteArrayContent(new byte[1024 * 1024]);
        var response = await _http.PostAsync(url, content);
        response.EnsureSuccessStatusCode(); // Status Code 200-299
    }

    [TestMethod]
    public async Task Ping()
    {
        var url = "/pINg";
        var response = await _http.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        Assert.AreEqual("[]", content);
    }

    [TestMethod]
    public async Task TestConnect()
    {
        var pusingUrl = $"ws://localhost:{_port}/Home/Pushing";
        var socket = await pusingUrl.ConnectAsWebSocketServer();
        await Task.Factory.StartNew(() => socket.Listen());

        var counter = new MessageCounter<string>();
        socket.Subscribe(counter);
        var lastStage = new MessageStageLast<string>();
        socket.Subscribe(lastStage);
        await Task.Delay(5000);
        await socket.Close();
        await Task.Delay(10);

        var latestTime = lastStage.Stage?.Split('|')[0];
        Assert.IsTrue(DateTime.TryParse(latestTime, out _), $"Got message {latestTime} is not a date time.");
        Assert.IsGreaterThan(30, counter.Count);
        Assert.IsLessThan(70, counter.Count);
    }

    private async Task<string> GetAntiCsrfToken(string url)
    {
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        var match = Regex.Match(html,
            @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" />");
        if (!match.Success)
        {
            throw new InvalidOperationException($"Could not find anti-CSRF token on page: {url}");
        }

        return match.Groups[1].Value;
    }

    [TestMethod]
    public async Task RegisterAndLoginAndLogOffTest()
    {
        var expectedUserName = $"test-{Guid.NewGuid()}";
        var email = $"{expectedUserName}@aiursoft.com";
        var password = "Test-Password-123";

        // Step 1: Register a new user and assert a successful redirect.
        var registerToken = await GetAntiCsrfToken("/Account/Register");
        var registerContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "Email", email },
            { "Password", password },
            { "ConfirmPassword", password },
            { "__RequestVerificationToken", registerToken }
        });
        var registerResponse = await _http.PostAsync("/Account/Register", registerContent);
        Assert.AreEqual(HttpStatusCode.Found, registerResponse.StatusCode);
        Assert.AreEqual("/Home/Index", registerResponse.Headers.Location?.OriginalString);

        // Step 2: Log off the user and assert a successful redirect.
        var homePageResponse = await _http.GetAsync("/Manage/Index");
        homePageResponse.EnsureSuccessStatusCode();
        var logOffToken = await GetAntiCsrfToken("/Manage/ChangePassword");
        var logOffContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "__RequestVerificationToken", logOffToken }
        });
        var logOffResponse = await _http.PostAsync("/Account/LogOff", logOffContent);
        Assert.AreEqual(HttpStatusCode.Found, logOffResponse.StatusCode);
        Assert.AreEqual("/", logOffResponse.Headers.Location?.OriginalString);

        // Step 3: Log in with the newly created user and assert a successful redirect.
        var loginToken = await GetAntiCsrfToken("/Account/Login");
        var loginContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "EmailOrUserName", email },
            { "Password", password },
            { "__RequestVerificationToken", loginToken }
        });
        var loginResponse = await _http.PostAsync("/Account/Login", loginContent);
        Assert.AreEqual(HttpStatusCode.Found, loginResponse.StatusCode);
        Assert.AreEqual("/Home/Index", loginResponse.Headers.Location?.OriginalString);

        // Step 4: Verify the final login state by checking the home page content.
        var finalHomePageResponse = await _http.GetAsync("/home/index");
        finalHomePageResponse.EnsureSuccessStatusCode();
        var finalHtml = await finalHomePageResponse.Content.ReadAsStringAsync();
        Assert.Contains(expectedUserName, finalHtml);
    }

    [TestMethod]
    public async Task LoginWithInvalidCredentialsTest()
    {
        // Step 1: Attempt to log in with credentials for a user that does not exist.
        var email = $"test-{Guid.NewGuid()}@aiursoft.com";
        var password = "Wrong-Password-123";
        var loginToken = await GetAntiCsrfToken("/Account/Login");
        var loginContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "EmailOrUserName", email },
            { "Password", password },
            { "__RequestVerificationToken", loginToken }
        });
        var loginResponse = await _http.PostAsync("/Account/Login", loginContent);

        // Step 2: Assert that the login fails and the correct error message is displayed.
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
        var html = await loginResponse.Content.ReadAsStringAsync();
        Assert.Contains("Invalid login attempt.", html);
    }

    [TestMethod]
    public async Task RegisterWithExistingEmailTest()
    {
        // Step 1: Register a new user successfully.
        var email = $"test-{Guid.NewGuid()}@aiursoft.com";
        var password = "Test-Password-123";
        var registerToken = await GetAntiCsrfToken("/Account/Register");
        var registerContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "Email", email },
            { "Password", password },
            { "ConfirmPassword", password },
            { "__RequestVerificationToken", registerToken }
        });
        var registerResponse = await _http.PostAsync("/Account/Register", registerContent);
        Assert.AreEqual(HttpStatusCode.Found, registerResponse.StatusCode);

        // Step 2: Log off to clear the current session.
        var logOffToken = await GetAntiCsrfToken("/Manage/ChangePassword");
        var logOffContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "__RequestVerificationToken", logOffToken }
        });
        await _http.PostAsync("/Account/LogOff", logOffContent);

        // Step 3: Attempt to register again using the same email.
        var secondRegisterToken = await GetAntiCsrfToken("/Account/Register");
        var secondRegisterContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "Email", email },
            { "Password", password },
            { "ConfirmPassword", password },
            { "__RequestVerificationToken", secondRegisterToken }
        });
        var secondRegisterResponse = await _http.PostAsync("/Account/Register", secondRegisterContent);

        // Step 4: Assert the registration fails and the correct error message is displayed.
        Assert.AreEqual(HttpStatusCode.OK, secondRegisterResponse.StatusCode);
        var html = await secondRegisterResponse.Content.ReadAsStringAsync();
        Assert.Contains("The username already exists. Please try another username.", html);
    }

    [TestMethod]
    public async Task LoginWithExistingUserAndWrongPasswordTest()
    {
        // Step 1: Register a new user.
        var email = $"test-{Guid.NewGuid()}@aiursoft.com";
        var correctPassword = "Test-Password-123";
        var wrongPassword = "Wrong-Password-456";
        var registerToken = await GetAntiCsrfToken("/Account/Register");
        var registerContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "Email", email },
            { "Password", correctPassword },
            { "ConfirmPassword", correctPassword },
            { "__RequestVerificationToken", registerToken }
        });
        var registerResponse = await _http.PostAsync("/Account/Register", registerContent);
        Assert.AreEqual(HttpStatusCode.Found, registerResponse.StatusCode);

        // Step 2: Log off the user.
        var logOffToken = await GetAntiCsrfToken("/Manage/ChangePassword");
        var logOffContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "__RequestVerificationToken", logOffToken }
        });
        await _http.PostAsync("/Account/LogOff", logOffContent);

        // Step 3: Attempt to log in with the correct email but a wrong password.
        var loginToken = await GetAntiCsrfToken("/Account/Login");
        var loginContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "EmailOrUserName", email },
            { "Password", wrongPassword },
            { "__RequestVerificationToken", loginToken }
        });
        var loginResponse = await _http.PostAsync("/Account/Login", loginContent);

        // Step 4: Assert the login fails and displays an error message.
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
        var html = await loginResponse.Content.ReadAsStringAsync();
        Assert.Contains("Invalid login attempt.", html);
    }

    [TestMethod]
    public async Task AccountLockoutTest()
    {
        // Using the default setting for max failed access attempts in ASP.NET Core Identity.
        const int maxFailedAccessAttempts = 5;

        // Step 1: Register a new user and then log off.
        var email = $"test-{Guid.NewGuid()}@aiursoft.com";
        var correctPassword = "Test-Password-123";
        var wrongPassword = "Wrong-Password-456";
        var registerToken = await GetAntiCsrfToken("/Account/Register");
        var registerContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "Email", email },
            { "Password", correctPassword },
            { "ConfirmPassword", correctPassword },
            { "__RequestVerificationToken", registerToken }
        });
        await _http.PostAsync("/Account/Register", registerContent);
        var logOffToken = await GetAntiCsrfToken("/Manage/ChangePassword");
        var logOffContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "__RequestVerificationToken", logOffToken }
        });
        await _http.PostAsync("/Account/LogOff", logOffContent);

        // Step 2: Attempt to log in with the wrong password multiple times to trigger lockout.
        HttpResponseMessage loginResponse = null!;
        for (int i = 0; i < maxFailedAccessAttempts; i++)
        {
            var loginToken = await GetAntiCsrfToken("/Account/Login");
            var loginContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "EmailOrUserName", email },
                { "Password", wrongPassword },
                { "__RequestVerificationToken", loginToken }
            });
            loginResponse = await _http.PostAsync("/Account/Login", loginContent);
        }

        // Step 3: Assert that the account is now locked.
        Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
        var html = await loginResponse.Content.ReadAsStringAsync();
        Assert.Contains("This account has been locked out, please try again later.", html);

        // Step 4: Verify that logging in with the correct password also fails while the account is locked.
        var finalLoginToken = await GetAntiCsrfToken("/Account/Login");
        var finalLoginContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "EmailOrUserName", email },
            { "Password", correctPassword },
            { "__RequestVerificationToken", finalLoginToken }
        });
        var finalLoginResponse = await _http.PostAsync("/Account/Login", finalLoginContent);
        var finalHtml = await finalLoginResponse.Content.ReadAsStringAsync();
        Assert.AreEqual(HttpStatusCode.OK, finalLoginResponse.StatusCode);
        Assert.Contains("This account has been locked out, please try again later.", finalHtml);
    }

    private async Task<(string email, string password)> RegisterAndLoginAsync()
    {
        var email = $"test-{Guid.NewGuid()}@aiursoft.com";
        var password = "Test-Password-123";

        // Step 1: Register the new user.
        var registerToken = await GetAntiCsrfToken("/Account/Register");
        var registerContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "Email", email },
            { "Password", password },
            { "ConfirmPassword", password },
            { "__RequestVerificationToken", registerToken }
        });
        var registerResponse = await _http.PostAsync("/Account/Register", registerContent);

        // Step 2: Assert registration was successful and we are logged in.
        Assert.AreEqual(HttpStatusCode.Found, registerResponse.StatusCode);

        return (email, password);
    }

    [TestMethod]
    public async Task ChangePasswordSuccessfullyTest()
    {
        // Step 1: Register and log in a new user, storing their credentials for the entire test.
        // CHANGE: Stored both email and oldPassword from the single call.
        var (email, oldPassword) = await RegisterAndLoginAsync();
        var newPassword = "New-Password-456";

        // Step 2: Get the anti-CSRF token from the Change Password page.
        var changePasswordToken = await GetAntiCsrfToken("/Manage/ChangePassword");

        // Step 3: Post the form to change the password.
        var changePasswordContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "OldPassword", oldPassword },
            { "NewPassword", newPassword },
            { "ConfirmPassword", newPassword },
            { "__RequestVerificationToken", changePasswordToken }
        });
        var changePasswordResponse = await _http.PostAsync("/Manage/ChangePassword", changePasswordContent);

        // Step 4: Assert the password change was successful and redirected correctly.
        Assert.AreEqual(HttpStatusCode.Found, changePasswordResponse.StatusCode);
        // Note: The controller redirects to Index, not the base /Manage path.
        Assert.AreEqual("/Manage?Message=ChangePasswordSuccess",
            changePasswordResponse.Headers.Location?.OriginalString);

        // Step 5: Log off to test the new password.
        var logOffToken = await GetAntiCsrfToken("/Manage/ChangePassword"); // Get a token from a valid authenticated page.
        var logOffContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "__RequestVerificationToken", logOffToken }
        });
        await _http.PostAsync("/Account/LogOff", logOffContent);

        // Step 6: Verify that the old password no longer works for the original user.
        var oldLoginToken = await GetAntiCsrfToken("/Account/Login");
        var oldLoginContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            // CHANGE: Used the saved 'email' variable.
            { "EmailOrUserName", email },
            { "Password", oldPassword },
            { "__RequestVerificationToken", oldLoginToken }
        });
        var oldLoginResponse = await _http.PostAsync("/Account/Login", oldLoginContent);
        Assert.AreEqual(HttpStatusCode.OK, oldLoginResponse.StatusCode);

        // Step 7: Verify that the new password works for the original user.
        var newLoginToken = await GetAntiCsrfToken("/Account/Login");
        var newLoginContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            // CHANGE: Used the saved 'email' variable again.
            { "EmailOrUserName", email },
            { "Password", newPassword },
            { "__RequestVerificationToken", newLoginToken }
        });
        var newLoginResponse = await _http.PostAsync("/Account/Login", newLoginContent);
        Assert.AreEqual(HttpStatusCode.Found, newLoginResponse.StatusCode);
    }

    [TestMethod]
    public async Task ChangeProfileSuccessfullyTest()
    {
        // Step 1: Register and log in a new user.
        var (email, _) = await RegisterAndLoginAsync();
        var originalUserName = email.Split('@')[0];
        var newUserName = $"new-name-{new Random().Next(1000, 9999)}";

        // Step 2: Get the anti-CSRF token from the Change Profile page.
        var changeProfileToken = await GetAntiCsrfToken("/Manage/ChangeProfile");

        // Step 3: Post the form to change the user's display name.
        var changeProfileContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "Name", newUserName },
            { "__RequestVerificationToken", changeProfileToken }
        });
        var changeProfileResponse = await _http.PostAsync("/Manage/ChangeProfile", changeProfileContent);

        // Step 4: Assert the profile change was successful and redirected correctly.
        Assert.AreEqual(HttpStatusCode.Found, changeProfileResponse.StatusCode);
        Assert.AreEqual("/Manage?Message=ChangeProfileSuccess", changeProfileResponse.Headers.Location?.OriginalString);

        // Step 5: Visit the home page and verify the new name is displayed.
        var homePageResponse = await _http.GetAsync("/Home/index");
        homePageResponse.EnsureSuccessStatusCode();
        var html = await homePageResponse.Content.ReadAsStringAsync();
        Assert.Contains(newUserName, html);
        Assert.DoesNotContain(originalUserName, html);
    }
}
