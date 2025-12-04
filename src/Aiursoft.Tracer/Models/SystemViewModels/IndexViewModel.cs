using Aiursoft.UiStack.Layout;

namespace Aiursoft.Tracer.Models.SystemViewModels;

public class IndexViewModel : UiStackLayoutViewModel
{
    public IndexViewModel()
    {
        PageTitle = "System Info";
    }

    public string? CountryName { get; set; }
    public string? CountryCode { get; set; }
}
