using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Tracer.Entities;

public class GlobalSetting
{
    [Key]
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string Key { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? Value { get; set; }
}
