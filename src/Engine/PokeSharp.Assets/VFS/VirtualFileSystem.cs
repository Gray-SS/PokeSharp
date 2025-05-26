namespace PokeSharp.Assets.VFS;

public sealed class VirtualFileSystem : IVirtualFileSystem
{
    private readonly Dictionary<string, IVirtualFileSystemProvider> _mountedProviders;

    public VirtualFileSystem()
    {
        _mountedProviders = new Dictionary<string, IVirtualFileSystemProvider>();
    }

    public IVirtualFile CreateFile(string virtualPath, bool overwrite = false)
    {
        (string scheme, string localPath) = ParseVirtualPath(virtualPath);
        IVirtualFileSystemProvider provider = GetProvider(scheme);

        return provider.CreateFile(localPath, overwrite);
    }

    public IVirtualDirectory CreateDirectory(string virtualPath)
    {
        (string scheme, string localPath) = ParseVirtualPath(virtualPath);
        IVirtualFileSystemProvider provider = GetProvider(scheme);

        return provider.CreateDirectory(localPath);
    }

    public StreamWriter OpenWrite(string virtualPath)
    {
        (string scheme, string localPath) = ParseVirtualPath(virtualPath);
        IVirtualFileSystemProvider provider = GetProvider(scheme);

        return provider.OpenWrite(localPath);
    }

    public StreamReader OpenRead(string virtualPath)
    {
        (string scheme, string localPath) = ParseVirtualPath(virtualPath);
        IVirtualFileSystemProvider provider = GetProvider(scheme);

        return provider.OpenRead(localPath);
    }

    public IVirtualFile? GetFile(string virtualPath)
    {
        (string scheme, string localPath) = ParseVirtualPath(virtualPath);
        IVirtualFileSystemProvider provider = GetProvider(scheme);

        return provider.GetFile(localPath);
    }

    public IVirtualDirectory? GetDirectory(string virtualPath)
    {
        (string scheme, string localPath) = ParseVirtualPath(virtualPath);
        IVirtualFileSystemProvider provider = GetProvider(scheme);

        return provider.GetDirectory(localPath);
    }

    public IEnumerable<IVirtualDirectory> GetMountedDirectories()
    {
        return _mountedProviders.Values.Select(x => x.RootDir);
    }

    public void Clear()
    {
        _mountedProviders.Clear();
    }

    public void Mount(string mountPoint, IVirtualFileSystemProvider provider)
    {
        string scheme = ExtractScheme(mountPoint);
        if (_mountedProviders.ContainsKey(scheme))
            throw new InvalidOperationException($"Scheme '{scheme}' is already used by another provider.");

        _mountedProviders[scheme] = provider;
    }

    public void Unmount(string mountPoint)
    {
        string scheme = ExtractScheme(mountPoint);
        _mountedProviders.Remove(scheme);
    }

    private IVirtualFileSystemProvider GetProvider(string scheme)
    {
        if (!_mountedProviders.TryGetValue(scheme, out IVirtualFileSystemProvider? provider))
            throw new InvalidOperationException($"No providers mounted for '{scheme}'");

        return provider;
    }

    private static (string Scheme, string LocalPath) ParseVirtualPath(string path)
    {
        string scheme = ExtractScheme(path);
        string localPath = ExtractLocalPath(scheme.Length, path);
        return (scheme, localPath);
    }

    private static string ExtractLocalPath(int schemeLength, ReadOnlySpan<char> path)
    {
        return new string(path[(schemeLength + 3)..]);
    }

    private static string ExtractScheme(ReadOnlySpan<char> path)
    {
        var schemeEnd = path.IndexOf("://");
        if (schemeEnd == -1)
            throw new ArgumentException($"Invalid virtual path: '{path}'. Missing scheme (e.g. disk://path/to/file, rom://path/to/dir, ect.)");

        return new string(path[..schemeEnd]);
    }
}
