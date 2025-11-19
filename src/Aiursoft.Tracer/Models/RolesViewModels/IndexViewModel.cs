using Aiursoft.UiStack.Layout;
using Microsoft.AspNetCore.Identity;

namespace Aiursoft.Tracer.Models.RolesViewModels;

public class IndexViewModel : UiStackLayoutViewModel
{
    public IndexViewModel()
    {
        PageTitle = "Roles";
    }

    public required List<IdentityRoleWithCount> Roles { get; init; }
}

public class IdentityRoleWithCount
{
    public required IdentityRole Role { get; init; }
    public required int UserCount { get; init; }
}
