
namespace PokeSharp.Assets.VFS;

public sealed class VirtualFile : VirtualEntry, IVirtualFile
{
    public override bool IsFile => true;
    public override bool IsDirectory => false;

    public VirtualFile(IVirtualFileSystemProvider provider, IVirtualDirectory? parentDir, string entryName) : base(provider, parentDir, entryName)
    {
    }

    public StreamReader OpenRead()
    {
        return Provider.OpenRead(VirtualPath);
    }

    public StreamWriter OpenWrite()
    {
        return Provider.OpenWrite(VirtualPath);
    }
}