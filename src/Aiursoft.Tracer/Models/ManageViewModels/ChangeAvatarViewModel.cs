using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Aiursoft.UiStack.Layout;

namespace Aiursoft.Tracer.Models.ManageViewModels;

public class ChangeAvatarViewModel : UiStackLayoutViewModel
{
    public ChangeAvatarViewModel()
    {
        PageTitle = "Change Avatar";
    }

    [NotNull]
    [Display(Name = "Avatar file")]
    [Required(ErrorMessage = "The avatar file is required.")]
    [RegularExpression(@"^Workspace/avatar.*", ErrorMessage = "The avatar file is invalid. Please upload it again.")]
    [MaxLength(150)]
    [MinLength(2)]
    public string? AvatarUrl { get; set; }
}

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
