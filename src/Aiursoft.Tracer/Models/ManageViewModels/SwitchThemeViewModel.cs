using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Tracer.Models.ManageViewModels;

public class SwitchThemeViewModel
{
    [Required]
    public required string Theme { get; set; }
}
