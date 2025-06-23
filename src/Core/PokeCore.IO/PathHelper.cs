using System.Runtime.InteropServices;

namespace PokeCore.IO;

public static class PathHelper
{
    public static string GetHomePath()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) :
            Environment.GetEnvironmentVariable("HOME") ?? "/home/" + Environment.UserName;
    }
}