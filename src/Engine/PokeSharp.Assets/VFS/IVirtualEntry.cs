namespace PokeSharp.Assets.VFS;

public interface IVirtualEntry
{
    string Name { get; }
    bool Exists { get; }
    bool IsDirectory { get; }
    public VirtualPath Path { get; }

    IVirtualFileSystemProvider Provider { get; }

    IVirtualDirectory GetParent();
    IVirtualEntry Rename(string name);
    IVirtualEntry Duplicate();
    bool Delete();
}