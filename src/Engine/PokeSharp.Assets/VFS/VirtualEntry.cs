using PokeSharp.Assets.VFS.Extensions;
using PokeSharp.Assets.VFS.Volumes;

namespace PokeSharp.Assets.VFS;

public abstract class VirtualEntry : IVirtualEntry, IEquatable<IVirtualEntry>
{
    public string Name => Path.IsRoot ? "Root" : Path.Name;
    public string NameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(Name);
    public long Size { get; }
    public VirtualPath Path { get; }
    public bool IsDirectory => Path.IsDirectory;
    public virtual bool Exists => Volume.AsFetchable().EntryExists(Path);

    public IVirtualVolume Volume { get; }

    public VirtualEntry(IVirtualVolume volume, VirtualPath path)
    {
        Path = path;
        Volume = volume;
    }

    public virtual IVirtualDirectory GetParent()
    {
        VirtualPath parentPath = Path.GetParent();
        return new VirtualDirectory(Volume, parentPath);
    }

    public virtual IVirtualEntry Rename(string name)
        => Volume.AsWriteable().RenameEntry(Path, name);

    public virtual IVirtualEntry Move(VirtualPath destPath)
        => Volume.AsWriteable().MoveEntry(Path, destPath);

    public virtual bool Delete()
        => Volume.AsWriteable().DeleteEntry(Path);

    public virtual IVirtualEntry Duplicate()
        => Volume.AsWriteable().DuplicateEntry(Path);

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