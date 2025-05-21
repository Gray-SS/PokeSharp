namespace PokeSharp.Core.Resolutions;


public readonly struct ResolutionSize : IEquatable<ResolutionSize>
{
    public int Width { get; }
    public int Height { get; }

    public float AspectRatio => (float)Width / Height;
    public bool IsLandscape => Width > Height;
    public bool IsPortrait => Height > Width;
    public bool IsSquare => Width == Height;

    public static readonly ResolutionSize R800x400 = new(800, 400);
    public static readonly ResolutionSize R1280x720 = new(1280, 720);
    public static readonly ResolutionSize R1920x1080 = new(1920, 1080);
    public static readonly ResolutionSize R3840x2160 = new(3840, 2160);
    public static readonly ResolutionSize R2560x1440 = new(2560, 1440);

    public ResolutionSize(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public bool Equals(ResolutionSize other)
    {
        return Width == other.Width && Height == other.Height;
    }

    public override bool Equals(object? obj)
    {
        return obj is ResolutionSize resolution && Equals(resolution);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    public override string ToString()
    {
        return $"{Width}x{Height}";
    }

    public static bool operator ==(ResolutionSize left, ResolutionSize right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ResolutionSize left, ResolutionSize right)
    {
        return !left.Equals(right);
    }
}