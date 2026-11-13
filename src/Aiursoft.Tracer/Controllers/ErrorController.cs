using System.Diagnostics;
using System.Globalization;
using Aiursoft.Tracer.Models.ErrorViewModels;
using Aiursoft.Tracer.Services;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Tracer.Controllers;

/// <summary>
/// This controller is used to show error pages.
/// </summary>
public class ErrorController : Controller
{
    [Route("Error/Code{code:int}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Code()
    {
        // Error responses must not bind the original request body, which may be malformed.
        var code = Convert.ToInt32(RouteData.Values["code"], CultureInfo.InvariantCulture);
        var returnUrl = Request.Query["returnUrl"].FirstOrDefault();
        var model = new ErrorViewModel(code)
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? null : returnUrl
        };

        return this.StackView(model, viewName: "Error");
    }
}
