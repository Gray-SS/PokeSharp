namespace PokeSharp.Core.Logging;

public sealed class EmptyLogger : ILogger
{
    public void Debug(string message)
    {
    }

    public void Error(string message)
    {
    }

    public void Fatal(string message)
    {
    }

    public void Info(string message)
    {
    }

    public void Log(LogLevel level, string message)
    {
    }

    public void Warn(string message)
    {
    }
}