namespace PokeEngine.Core.Resolutions;

public readonly struct ResolutionSize : IEquatable<ResolutionSize>
{
    public int Width { get; }
    public int Height { get; }

    public float AspectRatio => (float)Width / Height;
    public bool IsLandscape => Width > Height;
    public bool IsPortrait => Height > Width;
    public bool IsSquare => Width == Height;

    public static readonly ResolutionSize R800x600   = new(800, 600);
    public static readonly ResolutionSize R1280x720  = new(1280, 720);    // HD (16:9)
    public static readonly ResolutionSize R1280x960  = new(1280, 960);    // HD (16:9)
    public static readonly ResolutionSize R1366x768  = new(1366, 768);    // HD+ (16:9)
    public static readonly ResolutionSize R1440x900  = new(1440, 900);
    public static readonly ResolutionSize R1600x900  = new(1600, 900);    // HD+ (16:9)
    public static readonly ResolutionSize R1920x1080 = new(1920, 1080);   // Full HD (16:9)
    public static readonly ResolutionSize R2560x1440 = new(2560, 1440);   // Full HD (16:9)

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