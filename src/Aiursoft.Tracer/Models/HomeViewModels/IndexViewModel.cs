using Aiursoft.UiStack.Layout;

namespace Aiursoft.Tracer.Models.HomeViewModels;

public class IndexViewModel : UiStackLayoutViewModel
{
    public string? CountryName { get; set; }
    public string? CountryCode { get; set; }

    public IndexViewModel()
    {
        PageTitle = "Tester";
    }
}

public class SelfhostViewModel : UiStackLayoutViewModel
{
    public SelfhostViewModel()
    {
        PageTitle = "Self host your own server";
    }
}
