using Aiursoft.Tracer.Entities;
using Aiursoft.UiStack.Layout;

namespace Aiursoft.Tracer.Models.UsersViewModels;

public class DeleteViewModel : UiStackLayoutViewModel
{
    public DeleteViewModel()
    {
        PageTitle = "Delete User";
    }

    public required User User { get; set; }
}
