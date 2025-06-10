
namespace PokeEngine.Assets.Exceptions;

/// <summary>
/// Represents errors that occur during the import of assets in the asset pipeline.
/// </summary>
/// <remarks>
/// This exception is thrown when an asset fails to import correctly, providing details about the failure.
/// </remarks>
/// <seealso cref="AssetPipelineException"/>
public sealed class AssetImporterException : AssetPipelineException
{
    public AssetImporterException(string? message) : base(message)
    {
    }

    public AssetImporterException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}