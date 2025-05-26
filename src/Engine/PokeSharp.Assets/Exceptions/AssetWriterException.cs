
namespace PokeSharp.Assets.Exceptions;

public sealed class AssetWriterException : AssetPipelineException
{
    public AssetWriterException(string? message) : base(message)
    {
    }

    public AssetWriterException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}