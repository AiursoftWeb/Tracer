using Aiursoft.UiStack.Layout;

namespace Aiursoft.Tracer.Models.ManageViewModels;

public class IndexViewModel: UiStackLayoutViewModel
{
    public IndexViewModel()
    {
        PageTitle = "Manage";
    }

    public bool AllowUserAdjustNickname { get; set; }
}
