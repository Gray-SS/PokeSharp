using PokeEngine.Core.Exceptions;

namespace PokeEngine.Assets.Exceptions;

/// <summary>
/// Represents errors that occur during the asset pipeline processing in the engine.
/// </summary>
/// <remarks>
/// This exception is thrown when an error is encountered while processing assets,
/// such as loading, transforming, or managing asset data within the pipeline.
/// </remarks>
public class AssetPipelineException : EngineException
{
    public AssetPipelineException(string? message) : base(message)
    {
    }

    public AssetPipelineException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}