namespace PokeSharp.Assets.VFS;

public interface IVirtualFileSystem
{
    IEnumerable<IVirtualDirectory> GetMountedDirectories();

    IVirtualFile? GetFile(string virtualPath);
    IVirtualDirectory? GetDirectory(string virtualPath);

    IVirtualFile CreateFile(string virtualPath, bool overwrite = false);
    IVirtualDirectory CreateDirectory(string virtualPath);

    StreamWriter OpenWrite(string virtualPath);
    StreamReader OpenRead(string virtualPath);

    void Clear();
    void Mount(string scheme, IVirtualFileSystemProvider provider);
    void Unmount(string scheme);
}