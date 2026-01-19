using Aiursoft.Tracer.Models;

namespace Aiursoft.Tracer.Configuration;

public class SettingsMap
{
    public const string AllowUserAdjustNickname = "Allow_User_Adjust_Nickname";

    public class FakeLocalizer
    {
        public string this[string name] => name;
    }

    private static readonly FakeLocalizer Localizer = new();

    public static readonly List<GlobalSettingDefinition> Definitions = new()
    {
        new GlobalSettingDefinition
        {
            Key = AllowUserAdjustNickname,
            Name = Localizer["Allow User Adjust Nickname"],
            Description = Localizer["Allow users to adjust their nickname in the profile management page."],
            Type = SettingType.Bool,
            DefaultValue = "True"
        }
    };
}
