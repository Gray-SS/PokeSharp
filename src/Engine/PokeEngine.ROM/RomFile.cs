using PokeEngine.Assets.VFS;

namespace PokeEngine.ROM;

public sealed class RomFile : RomEntry
{
    public ReadOnlyMemory<byte> Data { get; }

    public RomFile(VirtualPath path, ReadOnlyMemory<byte> data) : base(path)
    {
        Data = data;
    }
}