
namespace PokeSharp.Assets.Exceptions;

/// <summary>
/// Represents errors that occur during asset processing in the asset pipeline.
/// </summary>
/// <remarks>
/// This exception is thrown when an error is encountered while processing assets,
/// typically within custom asset processors.
/// </remarks>
public sealed class AssetProcessorException : AssetPipelineException
{
    public AssetProcessorException(string? message) : base(message)
    {
    }

    public AssetProcessorException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}