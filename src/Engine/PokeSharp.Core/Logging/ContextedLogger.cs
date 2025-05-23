namespace PokeSharp.Core.Logging;

public sealed class ContextedLogger : ILogger
{
    private readonly string _context;
    private readonly LoggerSettings _settings;

    public ContextedLogger(LoggerSettings settings, string context)
    {
        _context = context;
        _settings = settings;
    }

    public void Log(LogLevel level, string message)
    {
        if (level < _settings.LogLevel)
            return;

        var timestamp = DateTime.UtcNow;
        var entry = new LogEntry(timestamp, level, _context, message);

        foreach (ILogOutput output in _settings.Outputs)
        {
            output.Log(in entry);
        }
    }

    public void Debug(string message)
    {
        Log(LogLevel.Debug, message);
    }

    public void Info(string message)
    {
        Log(LogLevel.Info, message);
    }

    public void Warn(string message)
    {
        Log(LogLevel.Warn, message);
    }

    public void Error(string message)
    {
        Log(LogLevel.Error, message);
    }

    public void Fatal(string message)
    {
        Log(LogLevel.Fatal, message);
    }
}