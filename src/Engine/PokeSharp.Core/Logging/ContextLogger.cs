namespace PokeSharp.Core.Logging;

public sealed class ContextLogger : ILogger
{
    private readonly string _context;
    private readonly LoggerSettings _settings;

    public ContextLogger(LoggerSettings settings, string context)
    {
        _context = context;
        _settings = settings;
    }

    public void Log(LogLevel level, string message)
    {
        if (level < _settings.LogLevel)
            return;

        var timestamp = DateTime.Now;
        var entry = new LogEntry(timestamp, level, _context, message);

        foreach (ILogOutput output in _settings.Outputs)
        {
            output.Log(in entry);
        }
    }

    public void Trace(string message)
    {
#if TRACE
        Log(LogLevel.Trace, message);
#endif
    }

    public void Debug(string message)
    {
#if DEBUG
        Log(LogLevel.Debug, message);
#endif
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