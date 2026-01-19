using Aiursoft.Tracer.Services.FileStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Aiursoft.Tracer.Views.Shared.Components.FileUpload;

public class FileUpload(StorageService storage) : ViewComponent
{
    public IViewComponentResult Invoke(
        ModelExpression aspFor,
        string subfolder,
        int maxSizeInMb = 2000,
        string? allowedExtensions = null,
        bool isVault = false)
    {
        var uploadEndpoint = storage.GetUploadUrl(subfolder, isVault);
        return View(new FileUploadViewModel
        {
            AspFor = aspFor,
            UploadEndpoint = uploadEndpoint,
            MaxSizeInMb = maxSizeInMb,
            AllowedExtensions = allowedExtensions,
            IsVault = isVault
        });
    }
}
