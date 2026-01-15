using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Aiursoft.Tracer.Views.Shared.Components.FileUpload;

public class FileUpload : ViewComponent
{
    public IViewComponentResult Invoke(
        ModelExpression aspFor,
        string uploadEndpoint,
        int maxSizeInMb = 2000,
        string? allowedExtensions = null)
    {
        return View(new FileUploadViewModel
        {
            AspFor = aspFor,
            UploadEndpoint = uploadEndpoint,
            MaxSizeInMb = maxSizeInMb,
            AllowedExtensions = allowedExtensions,
        });
    }
}
