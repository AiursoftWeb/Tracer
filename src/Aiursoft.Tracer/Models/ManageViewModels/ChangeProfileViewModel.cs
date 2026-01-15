using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Aiursoft.UiStack.Layout;

namespace Aiursoft.Tracer.Models.ManageViewModels;

public class ChangeProfileViewModel : UiStackLayoutViewModel
{
    public ChangeProfileViewModel()
    {
        PageTitle = "Change Profile";
    }

    [NotNull]
    [Display(Name = "Name")]
    [Required(ErrorMessage = "The name is required.")]
    [MaxLength(30)]
    [MinLength(2)]
    public string? Name { get; set; }
}
