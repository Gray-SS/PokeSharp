using PokeCore.Hosting.Exceptions;

namespace PokeEngine.Core.Exceptions;

public class EngineException : AppException
{
    public EngineException(string? message) : base(message) { }
    public EngineException(string? message, Exception? innerException) : base(message, innerException) {}
}