namespace PokeSharp.Engine.Assets.Exceptions;

public sealed class AssetProcessingException : AssetException
{
    public AssetProcessingException(string message) : base(message) { }
    public AssetProcessingException(string message, Exception innerException) : base(message, innerException) { }
}