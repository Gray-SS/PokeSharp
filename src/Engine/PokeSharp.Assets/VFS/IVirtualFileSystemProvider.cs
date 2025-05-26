namespace PokeSharp.Assets.VFS;

public interface IVirtualFileSystemProvider
{
    /// <summary>
    /// Mainly used for debugging
    /// </summary>
    string Name { get; }

    bool IsReadOnly { get; }
    IVirtualDirectory RootDir { get; }

    IVirtualFile? GetFile(string virtualPath);
    IVirtualDirectory? GetDirectory(string virtualPath);

    IVirtualFile CreateFile(string virtualPath, bool overwrite);
    IVirtualDirectory CreateDirectory(string virtualPath);

    StreamWriter OpenWrite(string virtualPath);
    StreamReader OpenRead(string virtualPath);
}