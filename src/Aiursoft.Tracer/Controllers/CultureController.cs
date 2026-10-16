using System.Globalization;
using Aiursoft.WebTools.Attributes;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Tracer.Controllers;

/// <summary>
/// This controller is used to change the current culture.
/// </summary>
[LimitPerMin]
public class CultureController : ControllerBase
{
    public IActionResult Set(string culture, string returnUrl)
    {
        if (string.IsNullOrEmpty(culture))
            return BadRequest("Culture cannot be null or empty.");

        RequestCulture requestCulture;
        try
        {
            requestCulture = new RequestCulture(culture);
        }
        catch (CultureNotFoundException)
        {
            return BadRequest("Culture is not supported.");
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(requestCulture),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}
