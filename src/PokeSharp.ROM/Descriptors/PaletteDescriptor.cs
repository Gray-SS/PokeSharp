using PokeSharp.ROM.Graphics;

namespace PokeSharp.ROM.Descriptors;

public readonly struct PaletteDescriptor : IRomDescriptor
{
    public int Offset { get; }
    public bool Compressed { get; }
    public RomPixelFormat PixelFormat { get; }
    public RomAssetType Type => RomAssetType.Palette;

    public PaletteDescriptor(int offset, bool compressed, RomPixelFormat pixelFormat)
    {
        Offset = offset;
        Compressed = compressed;
        PixelFormat = pixelFormat;
    }
}