using PokeSharp.Assets.VFS.Events;

namespace PokeSharp.Assets.VFS;

public interface IVirtualFileSystem
{
    event EventHandler<VolumeInfo>? OnVolumeMounted;
    event EventHandler<VolumeInfo>? OnVolumeUnmounted;
    event EventHandler<FileSystemChangedArgs>? OnFileChanged;

    bool Exists(VirtualPath virtualPath);
    bool FileExists(VirtualPath virtualPath);
    bool DirectoryExists(VirtualPath virtualPath);

    IVirtualEntry GetEntry(VirtualPath virtualPath);
    IVirtualFile GetFile(VirtualPath virtualPath);
    IVirtualDirectory GetDirectory(VirtualPath virtualPath);

    bool DeleteEntry(VirtualPath vpath);
    IVirtualEntry RenameEntry(VirtualPath vpath, string newName);
    IVirtualEntry DuplicateEntry(VirtualPath vpath);
    IVirtualEntry MoveEntry(VirtualPath vpath, VirtualPath newPath);

    IVirtualFile CreateFile(VirtualPath virtualPath, bool overwrite = false);
    IVirtualDirectory CreateDirectory(VirtualPath virtualPath);
    IEnumerable<IVirtualFile> GetFiles(VirtualPath virtualPath);
    IEnumerable<IVirtualDirectory> GetDirectories(VirtualPath virtualPath);

    Stream OpenWrite(VirtualPath virtualPath);
    Stream OpenRead(VirtualPath virtualPath);
    byte[] ReadBytes(VirtualPath virtualPath);

    bool IsVolumeMounted(string scheme);
    VolumeInfo GetVolume(string scheme);
    IEnumerable<VolumeInfo> GetVolumes();
    void MountVolume(VolumeInfo volume, IVirtualFileSystemProvider provider);
    void UnmountVolume(string scheme);
    void UnmountVolume(VolumeInfo volume);
    void UnmountVolumes();
}