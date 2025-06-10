using System.Diagnostics;
using PokeEngine.Assets.VFS.Extensions;
using PokeEngine.Assets.VFS.Volumes;

namespace PokeEngine.Assets.VFS;

public sealed class VirtualFile : VirtualEntry, IVirtualFile
{
    public VirtualFile(IVirtualVolume volume, VirtualPath path) : base(volume, path)
    {
        Debug.Assert(path.IsFile, $"Provided path doesn't represents a file: {path}");
    }

    public Stream OpenRead()
        => Volume.AsReadable().OpenRead(Path);

    public byte[] ReadBytes()
        => Volume.AsReadable().ReadBytes(Path);

    public Stream OpenWrite()
        => Volume.AsWriteable().OpenWrite(Path);
}