namespace Aiursoft.Tracer.Models;

public class GlobalSettingDefinition
{
    public required string Key { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required SettingType Type { get; init; }
    public required string DefaultValue { get; init; }
    public Dictionary<string, string>? ChoiceOptions { get; init; }
}
