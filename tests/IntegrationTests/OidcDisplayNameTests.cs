using System.Security.Claims;
using Aiursoft.Tracer.Configuration;
using Aiursoft.Tracer.Entities;
using Aiursoft.Tracer.Services.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Aiursoft.Tracer.Tests.IntegrationTests;

[TestClass]
[DoNotParallelize]
public class OidcDisplayNameTests : TestBase
{
    [TestMethod]
    [DataRow(null, true)]
    [DataRow("", true)]
    [DataRow(" \t\r\n", true)]
    [DataRow("  OIDC Display Name  ", false)]
    public async Task DisplayNameFallsBackToUsernameWhenBlank(string? displayName, bool useUsername)
    {
        using var scope = Server!.Services.CreateScope();
        var settings = scope.ServiceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
        var configuration = new ConfigurationBuilder()
            .AddConfiguration(scope.ServiceProvider.GetRequiredService<IConfiguration>())
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:AuthProvider"] = "OIDC",
                ["AppSettings:OIDC:Authority"] = "https://idp.example.com",
                ["AppSettings:OIDC:ClientId"] = "display-name-tests",
                ["AppSettings:OIDC:ClientSecret"] = "test-secret"
            }).Build();
        // 通过真实注册取得 OIDC 事件，复用现有测试宿主的数据库及业务隔离配置。
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDataProtection();
        services.AddTemplateAuth(configuration);
        await using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>()
            .Get(OpenIdConnectDefaults.AuthenticationScheme);
        var suffix = Guid.NewGuid().ToString("N");
        var username = $"oidc-{suffix}";
        var subject = $"subject-{suffix}";
        var claims = new List<Claim>
        {
            new(settings.OIDC.UsernamePropertyName, username),
            new(settings.OIDC.EmailPropertyName, $"{username}@example.com"),
            new(settings.OIDC.UserIdentityPropertyName, subject)
        };
        if (displayName != null)
        {
            claims.Add(new Claim(settings.OIDC.UserDisplayNamePropertyName, displayName));
        }
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        string? userId = null;
        // 同时覆盖新用户创建和同一 subject 已绑定用户的再次同步。
        for (var attempt = 0; attempt < 2; attempt++)
        {
            var context = new TokenValidatedContext(
                new DefaultHttpContext { RequestServices = scope.ServiceProvider },
                new AuthenticationScheme(OpenIdConnectDefaults.AuthenticationScheme, null, typeof(OpenIdConnectHandler)),
                options,
                new ClaimsPrincipal(new ClaimsIdentity(claims, "test")),
                new AuthenticationProperties());
            await options.Events.TokenValidated(context);
            Assert.IsFalse(context.Result is { Failure: not null });
            var user = await userManager.FindByLoginAsync(OpenIdConnectDefaults.AuthenticationScheme, subject);
            Assert.IsNotNull(user);
            Assert.AreEqual(useUsername ? username : displayName, user.DisplayName);
            Assert.AreEqual(username, user.UserName);
            if (userId != null)
            {
                Assert.AreEqual(userId, user.Id);
            }
            userId = user.Id;
        }
    }
}
