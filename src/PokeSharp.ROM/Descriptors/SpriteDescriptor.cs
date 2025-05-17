using PokeSharp.ROM.Graphics;

namespace PokeSharp.ROM.Descriptors;

public enum SpriteKind
{
    FrontPokemon,
    BackPokemon,
    ItemIcon,
    PokemonIcon,
}

public readonly struct SpriteDescriptor : IRomDescriptor
{
    public int Width { get; }
    public int Height { get; }
    public int Offset { get; }
    public int Length { get; }
    public bool Compressed { get; }
    public SpriteKind Kind { get; }
    public PaletteDescriptor Palette { get; }

    public RomPixelFormat PixelFormat { get; }
    public RomAssetType Type => RomAssetType.Sprite;

    public SpriteDescriptor(int offset, int width, int height, bool compressed, SpriteKind kind, PaletteDescriptor palette, RomPixelFormat pixelFormat)
    {
        if (palette.PixelFormat != pixelFormat)
            throw new InvalidOperationException($"Sprite descriptor is expecting a {pixelFormat} palette but received an {palette.PixelFormat} palette.");

        Width = width;
        Height = height;
        Kind = kind;
        Offset = offset;
        Palette = palette;
        Compressed = compressed;
        PixelFormat = pixelFormat;
    }
}