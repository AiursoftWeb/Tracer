using Aiursoft.UiStack.Layout;
using Microsoft.AspNetCore.Identity;

namespace Aiursoft.Tracer.Models.RolesViewModels;

public class DeleteViewModel : UiStackLayoutViewModel
{
    public DeleteViewModel()
    {
        PageTitle = "Delete Role";
    }

    public required IdentityRole Role { get; set; }
}
