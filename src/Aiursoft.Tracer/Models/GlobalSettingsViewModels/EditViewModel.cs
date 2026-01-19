using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Tracer.Models.GlobalSettingsViewModels;

public class EditViewModel
{
    [Required]
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
}
