using System.Diagnostics.CodeAnalysis;

namespace PokeSharp.Engine.UI;

public struct UIThickness : IEquatable<UIThickness>
{
    public int Left { get; set; }
    public int Right { get; set; }
    public int Top { get; set; }
    public int Bottom { get; set; }

    public readonly int Horizontal => Left + Right;
    public readonly int Vertical => Top + Bottom;

    public UIThickness(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public UIThickness(int horizontal, int vertical) : this(horizontal, vertical, horizontal, vertical)
    {
    }

    public UIThickness(int uniform) : this(uniform, uniform, uniform, uniform)
    {
    }

    public readonly bool Equals(UIThickness other)
    {
        return other.Left == Left && other.Right == Right && other.Top == Top && other.Bottom == Bottom;
    }

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is UIThickness other && Equals(other);
    }

    public static bool operator ==(UIThickness left, UIThickness right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(UIThickness left, UIThickness right)
    {
        return !(left == right);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Left, Right, Top, Bottom);
    }
}