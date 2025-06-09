namespace PokeSharp.Core.Modules.Exceptions;

public sealed class ModuleException : Exception
{
    public ModuleException(string? message) : base(message)
    {
    }

    public ModuleException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}