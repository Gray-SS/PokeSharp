namespace PokeSharp.Engine.Assets.Exceptions;

public sealed class AssetCacheException : AssetException
{
    public AssetCacheException(string message) : base(message) { }
    public AssetCacheException(string message, Exception innerException) : base(message, innerException) { }
}