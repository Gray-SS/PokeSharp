using System.Collections;

namespace PokeSharp.Assets.VFS;

public sealed class VirtualDirectory : VirtualEntry, IVirtualDirectory
{
    public override bool IsFile => false;
    public override bool IsDirectory => true;

    private readonly Dictionary<string, IVirtualEntry> _entries;

    public VirtualDirectory(IVirtualFileSystemProvider provider, IVirtualDirectory? parentDir, string dirName) : base(provider, parentDir, dirName)
    {
        _entries = new Dictionary<string, IVirtualEntry>();
    }

    public IVirtualFile CreateFile(string fileName, bool overwrite = false)
    {
        string virtualPath = $"{VirtualPath}/{fileName}";
        return Provider.CreateFile(virtualPath, overwrite);
    }

    public IVirtualDirectory CreateDirectory(string dirName)
    {
        string virtualPath = $"{VirtualPath}/{dirName}";
        return Provider.CreateDirectory(virtualPath);
    }

    public IVirtualDirectory GetDirectory(string path)
    {
        if (!_entries.TryGetValue(path, out IVirtualEntry? entry))
            throw new DirectoryNotFoundException($"Directory at path '{path}' wasn't found.");

        if (entry.IsFile)
            throw new InvalidOperationException($"The path lead to a file. Not a directory.");

        return (IVirtualDirectory)entry;
    }

    public IEnumerable<IVirtualEntry> GetEntries()
    {
        return _entries.Values;
    }

    public IEnumerator<IVirtualEntry> GetEnumerator()
    {
        return _entries.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}