using Aiursoft.UiStack.Layout;

namespace Aiursoft.Tracer.Models.ErrorViewModels;

public class ErrorViewModel: UiStackLayoutViewModel
{
    [Obsolete("Must use the constructor that takes the error code.")]
    public ErrorViewModel()
    {
        PageTitle = "Error";
    }

    public ErrorViewModel(int code)
    {
        ErrorCode = code;
        PageTitle = code switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Access Denied",
            404 => "Not Found",
            500 => "Internal Server Error",
            _ => $"Error {code}"
        };
    }

    public int ErrorCode { get; set; } = 500;

    public required string RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public string? ReturnUrl { get; set; }
}
