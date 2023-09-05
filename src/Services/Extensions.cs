using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Aiursoft.Tracer.Services;

public static class Extensions
{
    public static string TryGetFullOsVersion()
    {
        var osVer = Environment.OSVersion;
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return osVer.VersionString;

        try
        {
            var currentVersion = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            if (currentVersion != null)
            {
                var name = currentVersion.GetValue("ProductName", "Microsoft Windows NT");
                var ubr = currentVersion.GetValue("UBR", string.Empty).ToString();
                if (!string.IsNullOrWhiteSpace(ubr))
                    return $"{name} {osVer.Version.Major}.{osVer.Version.Minor}.{osVer.Version.Build}.{ubr}";
            }
        }
        catch
        {
            return osVer.VersionString;
        }

        return osVer.VersionString;
    }
}
