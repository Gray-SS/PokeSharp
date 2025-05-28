using PokeSharp.Assets.VFS.Events;

namespace PokeSharp.Assets.VFS;

public interface IVirtualFileSystem
{
    event EventHandler<VolumeInfo>? OnVolumeMounted;
    event EventHandler<VolumeInfo>? OnVolumeUnmounted;
    event EventHandler<FileSystemChangedArgs>? OnFileChanged;

    VolumeInfo GetVolume(string scheme);
    IEnumerable<VolumeInfo> GetVolumes();

    bool Exists(VirtualPath virtualPath);

    IVirtualFile GetFile(VirtualPath virtualPath);
    IVirtualDirectory GetDirectory(VirtualPath virtualPath);

    IVirtualFile CreateFile(VirtualPath virtualPath, bool overwrite = false);
    IVirtualDirectory CreateDirectory(VirtualPath virtualPath);
    IEnumerable<IVirtualFile> GetFiles(VirtualPath virtualPath);
    IEnumerable<IVirtualDirectory> GetDirectories(VirtualPath virtualPath);

    StreamWriter OpenWrite(VirtualPath virtualPath);
    StreamReader OpenRead(VirtualPath virtualPath);

    void MountVolume(VolumeInfo volume, IVirtualFileSystemProvider provider);
    void UnmountVolume(string scheme);
    void UnmountVolume(VolumeInfo volume);
    void UnmountVolumes();
}