using Aiursoft.Tracer.Authorization;
using Aiursoft.Tracer.Services;
using Aiursoft.UiStack.Navigation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Tracer.Models.SystemViewModels;
using Aiursoft.WebTools.Attributes;

namespace Aiursoft.Tracer.Controllers;

/// <summary>
/// This controller is used to handle system related actions like shutdown.
/// </summary>
[LimitPerMin]
public class SystemController(
    ILogger<SystemController> logger,
    IPGeolocationService ipGeolocationService) : Controller
{
    [RenderInNavBar(
        NavGroupName = "Administration",
        NavGroupOrder = 9999,
        CascadedLinksGroupName = "System",
        CascadedLinksIcon = "cog",
        CascadedLinksOrder = 9999,
        LinkText = "Info",
        LinkOrder = 1)]
    public async Task<IActionResult> Index()
    {
        var model = new IndexViewModel();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var location = await ipGeolocationService.GetLocationAsync(ip);
        if (location != null)
        {
            model.CountryName = location.Value.CountryName;
            model.CountryCode = location.Value.CountryCode;
        }
        return this.StackView(model);
    }

    [HttpPost]
    [Authorize]
    [Authorize(Policy = AppPermissionNames.CanRebootThisApp)] // Use the specific permission
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult Shutdown([FromServices] IHostApplicationLifetime appLifetime)
    {
        logger.LogWarning("Application shutdown was requested by user: '{UserName}'", User.Identity?.Name);
        appLifetime.StopApplication();
        return Accepted();
    }
}
