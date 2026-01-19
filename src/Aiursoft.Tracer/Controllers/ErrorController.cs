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
        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            ErrorCode = code,
            ReturnUrl = returnUrl
        };

        switch (code)
        {
            case 400:
                model.PageTitle = "Bad Request";
                break;
            case 401:
                model.PageTitle = "Unauthorized";
                break;
            case 403:
                model.PageTitle = "Access Denied"; // Changed from Forbidden to match "Access Denied" context usually
                break;
            case 404:
                model.PageTitle = "Not Found";
                break;
            case 500:
                model.PageTitle = "Internal Server Error";
                break;
            default:
                model.PageTitle = $"Error {code}";
                break;
        }

        return this.StackView(model, viewName: "Error");
    }
}
