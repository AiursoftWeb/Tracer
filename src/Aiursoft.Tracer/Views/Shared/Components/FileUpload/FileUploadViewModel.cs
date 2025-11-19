using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Aiursoft.Tracer.Views.Shared.Components.FileUpload;

public class FileUploadViewModel
{
    public required ModelExpression AspFor { get; init; }
    public required string UploadEndpoint { get; init; }
    public required int MaxSizeInMb { get; init; }
    public required string? AllowedExtensions { get; init; }
    public string UniqueId { get; } = "uploader-" + Guid.NewGuid().ToString("N");
}
