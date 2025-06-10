using PokeEngine.Assets.VFS.Volumes;

namespace PokeEngine.Assets.VFS;

public interface IVirtualEntry : IEquatable<IVirtualEntry>
{
    string Name { get; }
    string NameWithoutExtension { get; }
    long Size { get; }
    bool Exists { get; }
    bool IsDirectory { get; }
    public VirtualPath Path { get; }

    IVirtualVolume Volume { get; }

    IVirtualDirectory GetParent();
    IVirtualEntry Rename(string name);
    IVirtualEntry Move(VirtualPath newPath);
    IVirtualEntry Duplicate();
    bool Delete();
}