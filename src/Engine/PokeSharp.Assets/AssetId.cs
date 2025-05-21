namespace PokeSharp.Assets;

public readonly struct AssetId(string value) : IEquatable<AssetId>
{
    public readonly string Value = value;

    public bool Equals(AssetId other)
    {
        return other.Value == Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is AssetId id && Equals(id);
    }

    public static bool operator ==(AssetId left, AssetId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(AssetId left, AssetId right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}