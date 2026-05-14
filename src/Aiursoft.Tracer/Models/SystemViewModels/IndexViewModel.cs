using Aiursoft.UiStack.Layout;

namespace Aiursoft.Tracer.Models.SystemViewModels;

public class MigrationEntry
{
    public required string Id { get; init; }
    public string Name => Id.Length > 15 ? Id[15..] : Id;
    public DateTime? AppliedAt => Id.Length >= 14 && DateTime.TryParseExact(
        Id[..14], "yyyyMMddHHmmss",
        System.Globalization.CultureInfo.InvariantCulture,
        System.Globalization.DateTimeStyles.AssumeUniversal,
        out var dt) ? dt.ToUniversalTime() : null;
}

public class IndexViewModel : UiStackLayoutViewModel
{
    public IndexViewModel()
    {
        PageTitle = "System Info";
    }

    public string? CountryName { get; set; }
    public string? CountryCode { get; set; }
    public Dictionary<string, long> TableCounts { get; init; } = new();
    public List<MigrationEntry> AppliedMigrations { get; init; } = [];
    public int TotalDefinedMigrations { get; init; }
    public List<string> PendingMigrations { get; init; } = [];
}
