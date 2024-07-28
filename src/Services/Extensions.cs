using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Aiursoft.Tracer.Services;

public static class Extensions
{
    [ExcludeFromCodeCoverage]
    public static async Task<string> TryGetFullOsVersionAsync()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var osInfo = await File.ReadAllTextAsync("/etc/lsb-release");

                var prettyName = osInfo
                    .Split('\n')
                    .FirstOrDefault(l => l.StartsWith("DISTRIB_DESCRIPTION"))?
                    .Split('=')[1]
                    .Trim('"') ?? "Unknown";

                return prettyName.Trim();

            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var currentVersion = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                if (currentVersion != null)
                {
                    var name = currentVersion.GetValue("ProductName", "Microsoft Windows NT");
                    var ubr = currentVersion.GetValue("UBR", string.Empty).ToString();
                    var osVer = Environment.OSVersion;
                    if (!string.IsNullOrWhiteSpace(ubr))
                        return $"{name} {osVer.Version.Major}.{osVer.Version.Minor}.{osVer.Version.Build}.{ubr}";
                }
            }
        }
        catch
        {
            // ignored
        }

        return Environment.OSVersion.VersionString;
    }
}
