using System.Diagnostics.CodeAnalysis;
using PokeSharp.ROM.Graphics;

namespace PokeSharp.ROM.Descriptors;

public readonly struct RomPaletteDescriptor : IRomDescriptor, IEquatable<RomPaletteDescriptor>
{
    public int Offset { get; }
    public bool Compressed { get; }
    public RomPixelFormat PixelFormat { get; }
    public RomAssetType Type => RomAssetType.Palette;

    public RomPaletteDescriptor(int offset, bool compressed, RomPixelFormat pixelFormat)
    {
        Offset = offset;
        Compressed = compressed;
        PixelFormat = pixelFormat;
    }

    public bool Equals(RomPaletteDescriptor other)
    {
        return other.Offset == Offset && other.Compressed == Compressed && other.PixelFormat == PixelFormat;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is RomPaletteDescriptor other && Equals(other);
    }

    public static bool operator ==(RomPaletteDescriptor left, RomPaletteDescriptor right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RomPaletteDescriptor left, RomPaletteDescriptor right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Offset, Compressed, PixelFormat);
    }
}