namespace PokeSharp.Assets.VFS;

public abstract class VirtualEntry : IVirtualEntry, IEquatable<IVirtualEntry>
{
    public string Name => Path.IsRoot ? "Root" : Path.Name;
    public string NameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(Name);
    public long Size { get; }
    public VirtualPath Path { get; }
    public bool IsDirectory => Path.IsDirectory;
    public virtual bool Exists => Provider.Exists(Path);

    public IVirtualFileSystemProvider Provider { get; }

    public VirtualEntry(IVirtualFileSystemProvider provider, VirtualPath path)
    {
        Path = path;
        Provider = provider;
    }

    public virtual IVirtualDirectory GetParent()
    {
        VirtualPath parentPath = Path.GetParent();
        return new VirtualDirectory(Provider, parentPath);
    }

    public virtual IVirtualEntry Rename(string name)
    {
        return Provider.RenameEntry(Path, name);
    }

    public virtual IVirtualEntry Move(VirtualPath newPath)
    {
        return Provider.MoveEntry(Path, newPath);
    }

    public virtual bool Delete()
    {
        return Provider.DeleteEntry(Path);
    }

    public virtual IVirtualEntry Duplicate()
    {
        return Provider.DuplicateEntry(Path);
    }

    public bool Equals(IVirtualEntry? other)
    {
        return other is IVirtualEntry entry && entry.Path == Path;
    }

    public override bool Equals(object? obj)
    {
        return obj is IVirtualEntry entry && Equals(entry);
    }

    public static bool operator ==(VirtualEntry? left, object? right)
    {
        if (left is null) return right is null;

        return left.Equals(right);
    }

    public static bool operator !=(VirtualEntry? left, object? right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return Path.GetHashCode();
    }
}