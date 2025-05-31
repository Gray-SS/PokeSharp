
namespace PokeSharp.Assets.VFS;

public sealed class VirtualFile : VirtualEntry, IVirtualFile
{
    public VirtualFile(IVirtualFileSystemProvider provider, VirtualPath path) : base(provider, path)
    {
    }

    public Stream OpenRead()
    {
        return Provider.OpenRead(Path);
    }

    public byte[] ReadBytes()
    {
        return Provider.ReadBytes(Path);
    }

    public Stream OpenWrite()
    {
        return Provider.OpenWrite(Path);
    }
}