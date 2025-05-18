namespace PokeSharp.Engine.Assets.Exceptions;

public sealed class AssetImportException : AssetException
{
    public AssetImportException(string message) : base(message) { }
    public AssetImportException(string message, Exception innerException) : base(message, innerException) { }
}