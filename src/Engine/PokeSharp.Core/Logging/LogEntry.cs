namespace PokeSharp.Core.Logging;

public readonly struct LogEntry
{
    public readonly DateTime TimeStamp;
    public readonly LogLevel Level;
    public readonly string Context;
    public readonly string Message;

    public LogEntry(DateTime timeStamp, LogLevel level, string context, string message)
    {
        TimeStamp = timeStamp;
        Level = level;
        Context = context;
        Message = message;
    }
}