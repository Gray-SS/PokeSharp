namespace PokeSharp.Assets.VFS;

public interface IVirtualEntry
{
    string Name { get; }
    string VirtualPath { get; }

    IVirtualDirectory? ParentDirectory { get; }

    bool IsReadOnly { get; }

    bool IsFile { get; }
    bool IsDirectory { get; }

    IVirtualFileSystemProvider Provider { get; }
}