using PokeSharp.Assets.VFS;

namespace PokeSharp.ROM.Platforms.Gba;

public sealed class GbaFileSystemProvider : IVirtualFileSystemProvider
{
    public string Name => throw new NotImplementedException();
    public bool IsReadOnly => throw new NotImplementedException();

    public IVirtualDirectory RootDir => throw new NotImplementedException();

    public IVirtualDirectory CreateDirectory(string virtualPath)
    {
        throw new NotImplementedException();
    }

    public IVirtualFile CreateFile(string virtualPath, bool overwrite)
    {
        throw new NotImplementedException();
    }

    public IVirtualDirectory? GetDirectory(string virtualPath)
    {
        throw new NotImplementedException();
    }

    public IVirtualFile? GetFile(string virtualPath)
    {
        throw new NotImplementedException();
    }

    public StreamReader OpenRead(string virtualPath)
    {
        throw new NotImplementedException();
    }

    public StreamWriter OpenWrite(string virtualPath)
    {
        throw new NotImplementedException();
    }
}