using PokeSharp.Assets.VFS.Events;

namespace PokeSharp.Assets.VFS;

public interface IVirtualFileSystemProvider
{
    event EventHandler<FileSystemChangedArgs>? OnFileChanged;

    bool Exists(VirtualPath virtualPath);

    IVirtualFile GetFile(VirtualPath virtualPath);
    IVirtualDirectory GetDirectory(VirtualPath virtualPath);

    IEnumerable<IVirtualFile> GetFiles(VirtualPath virtualPath);
    IEnumerable<IVirtualDirectory> GetDirectories(VirtualPath virtualPath);

    bool DeleteEntry(VirtualPath virtualPath);
    IVirtualEntry RenameEntry(VirtualPath virtualPath, string name);
    IVirtualEntry DuplicateEntry(VirtualPath virtualPath);

    IVirtualFile CreateFile(VirtualPath virtualPath, bool overwrite);
    IVirtualDirectory CreateDirectory(VirtualPath virtualPath);

    StreamWriter OpenWrite(VirtualPath virtualPath);
    StreamReader OpenRead(VirtualPath virtualPath);
}