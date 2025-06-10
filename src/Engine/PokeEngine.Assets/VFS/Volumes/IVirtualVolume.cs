namespace PokeEngine.Assets.VFS.Volumes;

public interface IVirtualVolume
{
    string Id { get; }
    string Scheme { get; }
    string DisplayName { get; }

    VirtualPath RootPath { get; }
    IVirtualDirectory RootDirectory { get; }

    bool IsReadOnly { get; }
    bool CanFetch { get; }
    bool CanWrite { get; }
    bool CanRead { get; }
    bool CanWatch { get; }
    VolumeAccess AuthorizedAccesses { get; }

    bool HasAccess(VolumeAccess access);
}