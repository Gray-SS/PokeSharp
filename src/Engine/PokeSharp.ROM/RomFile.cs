using PokeSharp.Assets.VFS;

namespace PokeSharp.ROM;

public sealed class RomFile : RomEntry
{
    public ReadOnlyMemory<byte> Data { get; }

    public RomFile(VirtualPath path, ReadOnlyMemory<byte> data) : base(path)
    {
        Data = data;
    }
}