using System.Net;
using System.Text.RegularExpressions;
using Aiursoft.DbTools;
using Aiursoft.Tracer.Entities;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Tracer.Tests.IntegrationTests;

public abstract class TestBase
{
    protected readonly HttpClient Http;
    protected IHost? Server;
    private string? _storagePath;

    protected TestBase()
    {
        var cookieContainer = new CookieContainer();
        var handler = new HttpClientHandler
        {
            CookieContainer = cookieContainer,
            AllowAutoRedirect = false
        };
        Http = new HttpClient(handler);
    }

    [TestInitialize]
    public virtual async Task CreateServer()
    {
        using var reservation = new TestPortReservation();
        Http.BaseAddress = new Uri($"http://localhost:{reservation.Port}");
        _storagePath = Directory.CreateTempSubdirectory("AiursoftTests-").FullName;
        try
        {
            Server = await AppAsync<Startup>([$"Storage:Path={_storagePath}"], port: reservation.Port);
            reservation.ConfigureServer(Server);
            await Server.UpdateDbAsync<TracerDbContext>();
            await Server.SeedAsync();
            await Server.StartAsync();
        }
        catch
        {
            Server?.Dispose();
            Server = null;
            Directory.Delete(_storagePath, recursive: true);
            throw;
        }
    }

    [TestCleanup]
    public virtual async Task CleanServer()
    {
        try
        {
            if (Server != null)
            {
                await Server.StopAsync();
            }
        }
        finally
        {
            Server?.Dispose();
            Server = null;
            if (_storagePath != null && Directory.Exists(_storagePath))
            {
                Directory.Delete(_storagePath, recursive: true);
            }
        }
    }

    protected async Task<string> GetAntiCsrfToken(string url)
    {
        var response = await Http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            response = await Http.GetAsync("/");
        }
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

    protected async Task<HttpResponseMessage> PostForm(string url, Dictionary<string, string> data, string? tokenUrl = null, bool includeToken = true)
    {
        if (includeToken && !data.ContainsKey("__RequestVerificationToken"))
        {
            var token = await GetAntiCsrfToken(tokenUrl ?? url);
            data["__RequestVerificationToken"] = token;
        }
        return await Http.PostAsync(url, new FormUrlEncodedContent(data));
    }

    protected void AssertRedirect(HttpResponseMessage response, string expectedLocation, bool exact = true)
    {
        Assert.AreEqual(HttpStatusCode.Found, response.StatusCode);
        var actualLocation = response.Headers.Location?.OriginalString ?? string.Empty;
        var baseUri = Http.BaseAddress?.ToString() ?? "____";
        
        if (actualLocation.StartsWith(baseUri))
        {
            actualLocation = actualLocation.Substring(baseUri.Length - 1); // Keep the leading slash
        }

        if (exact)
        {
            Assert.AreEqual(expectedLocation, actualLocation, $"Expected redirect to {expectedLocation}, but was {actualLocation}");
        }
        else
        {
            Assert.StartsWith(expectedLocation, actualLocation);
        }
    }

    protected async Task LoginAsAdmin()
    {
        var loginResponse = await PostForm("/Account/Login", new Dictionary<string, string>
        {
            { "EmailOrUserName", "admin@default.com" },
            { "Password", "Admin@123456!" }
        });
        Assert.AreEqual(HttpStatusCode.Found, loginResponse.StatusCode);
    }

    protected async Task<(string email, string password)> RegisterAndLoginAsync()
    {
        var email = $"test-{Guid.NewGuid()}@aiursoft.com";
        var password = "Test-Password-123";

        var registerResponse = await PostForm("/Account/Register", new Dictionary<string, string>
        {
            { "Email", email },
            { "Password", password },
            { "ConfirmPassword", password }
        });
        Assert.AreEqual(HttpStatusCode.Found, registerResponse.StatusCode);

        return (email, password);
    }

    protected T GetService<T>() where T : notnull
    {
        if (Server == null) throw new InvalidOperationException("Server is not started.");
        return Server.Services.GetRequiredService<T>();
    }
}
