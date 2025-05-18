namespace PokeSharp.Engine.Assets;

public readonly struct AssetReference : IEquatable<AssetReference>
{
    public object Payload { get; }
    public AssetSource Source { get; }

    public AssetReference(AssetSource source, object payload)
    {
        Source = source;
        Payload = payload;
    }

    public T PayloadAs<T>()
    {
        return (T)Payload;
    }

    public bool Equals(AssetReference other)
    {
        return other.Source == Source && DeepEquals(Payload, other.Payload);
    }

    private static bool DeepEquals(object left, object right)
    {
        if (left == right)
            return true;

        return left.Equals(right);
    }

    public override bool Equals(object? obj)
    {
        return obj is AssetReference assetRef && Equals(assetRef);
    }

    public static bool operator ==(AssetReference left, AssetReference right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(AssetReference left, AssetReference right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Payload, Source);
    }
}