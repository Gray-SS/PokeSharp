namespace PokeSharp.Assets.VFS;

public abstract class VirtualEntry : IVirtualEntry
{
    public string Name => Path.IsRoot ? "Root" : Path.Name;
    public bool IsDirectory => Path.IsDirectory;
    public bool Exists => Provider.Exists(Path);
    public VirtualPath Path { get; }

    public IVirtualFileSystemProvider Provider { get; }

    public VirtualEntry(IVirtualFileSystemProvider provider, VirtualPath path)
    {
        Path = path;
        Provider = provider;
    }

    public IVirtualDirectory GetParent()
    {
        VirtualPath parentPath = Path.GetParent();
        return new VirtualDirectory(Provider, parentPath);
    }

    public IVirtualEntry Rename(string name)
    {
        return Provider.RenameEntry(Path, name);
    }

    public bool Delete()
    {
        return Provider.DeleteEntry(Path);
    }

    public IVirtualEntry Duplicate()
    {
        return Provider.DuplicateEntry(Path);
    }
}