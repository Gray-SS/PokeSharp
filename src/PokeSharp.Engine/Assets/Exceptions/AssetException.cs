namespace PokeSharp.Engine.Assets.Exceptions;

public class AssetException : Exception
{
    public AssetException(string message) : base(message) { }
    public AssetException(string message, Exception innerException) : base(message, innerException) { }
}