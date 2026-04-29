using System.Diagnostics;
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
    public IActionResult Code(int code, [FromQuery] string? returnUrl = null)
    {
        var model = new ErrorViewModel(code)
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            ReturnUrl = returnUrl
        };

        return this.StackView(model, viewName: "Error");
    }
}
