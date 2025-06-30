using PokeCore.Diagnostics;

namespace PokeCore.IO;

public static class VirtualPathHelper
{
    public static VirtualPath ResolvePhysicalPath(string path)
    {
        ThrowHelper.AssertNotNullOrWhitespace(path, "Path must be not null or empty");

        if (path.StartsWith("./"))
            return new VirtualPath("cwd", path.Substring(startIndex: 2));

        string fullPath = Path.GetFullPath(path);
        string homePath = PathHelper.GetHomePath();
        if (path.StartsWith("~/"))
        {
            path = fullPath.Substring(startIndex: 2);
            return new VirtualPath("home", path);
        }

        if (fullPath.StartsWith(homePath))
        {
            path = fullPath.Replace(homePath, string.Empty);
            return new VirtualPath("home", path);
        }

        throw new InvalidOperationException($"Path coulnd't be resolved");
    }
}