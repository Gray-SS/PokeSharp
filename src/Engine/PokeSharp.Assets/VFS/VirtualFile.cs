
namespace PokeSharp.Assets.VFS;

public sealed class VirtualFile : VirtualEntry, IVirtualFile
{
    public VirtualFile(IVirtualFileSystemProvider provider, VirtualPath path) : base(provider, path)
    {
    }

    public StreamReader OpenRead()
    {
        return Provider.OpenRead(Path);
    }

    public StreamWriter OpenWrite()
    {
        return Provider.OpenWrite(Path);
    }
}