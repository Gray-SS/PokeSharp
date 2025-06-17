using System.Diagnostics;
using PokeCore.IO.Extensions;
using PokeCore.IO.Volumes;

namespace PokeCore.IO;

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