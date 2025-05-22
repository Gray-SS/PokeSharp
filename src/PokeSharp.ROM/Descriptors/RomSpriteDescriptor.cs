using System.Diagnostics.CodeAnalysis;
using PokeSharp.ROM.Graphics;

namespace PokeSharp.ROM.Descriptors;

public readonly struct RomSpriteDescriptor : IRomDescriptor, IEquatable<RomSpriteDescriptor>
{
    public int Width { get; }
    public int Height { get; }
    public int Offset { get; }
    public bool Compressed { get; }
    public RomPaletteDescriptor Palette { get; }
    public RomPixelFormat PixelFormat { get; }

    public RomAssetType Type => RomAssetType.Sprite;

    public RomSpriteDescriptor(int offset, int width, int height, bool compressed, RomPaletteDescriptor palette, RomPixelFormat pixelFormat)
    {
        if (palette.PixelFormat != pixelFormat)
            throw new InvalidOperationException($"Sprite descriptor is expecting a {pixelFormat} palette but received an {palette.PixelFormat} palette.");

        Width = width;
        Height = height;
        Offset = offset;
        Palette = palette;
        Compressed = compressed;
        PixelFormat = pixelFormat;
    }

    public bool Equals(RomSpriteDescriptor other)
    {
        return other.Width == Width && other.Height == Height && other.Compressed == Compressed &&
            other.Offset == Offset && other.PixelFormat == PixelFormat && other.Palette == Palette;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is RomSpriteDescriptor other && Equals(other);
    }

    public static bool operator ==(RomSpriteDescriptor left, RomSpriteDescriptor right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RomSpriteDescriptor left, RomSpriteDescriptor right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height, Offset, Compressed, Palette, PixelFormat);
    }
}