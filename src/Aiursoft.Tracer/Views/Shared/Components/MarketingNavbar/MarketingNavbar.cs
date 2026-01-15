using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Tracer.Views.Shared.Components.MarketingNavbar;

public class MarketingNavbar : ViewComponent
{
    public IViewComponentResult Invoke(MarketingNavbarViewModel? model = null)
    {
        model ??= new MarketingNavbarViewModel();
        return View(model);
    }
}
