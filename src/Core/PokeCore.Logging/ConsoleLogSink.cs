using System.Diagnostics;

namespace PokeCore.Logging;

/// <summary>
/// Provides an implementation of a console log sink that writes log entries to the appropriate standard output.
/// </summary>
/// <remarks>
/// <para>
/// Depending on the <see cref="LogLevel"/> of each <see cref="LogEntry"/>, this sink will write:
/// <list type="bullet">
///   <item><description><c>stdout</c> for warning, informational, debug, or trace messages (levels &lt;= <see cref="LogLevel.Warn"/>).</description></item>
///   <item><description><c>stderr</c> for error, or fatal messages (levels &gt;= <see cref="LogLevel.Error"/>).</description></item>
/// </list>
/// </para>
/// <para>
/// Each entry is formatted (timestamp, context, level, message) before being written.
/// Useful in the runtime application or more generally if no UI presentations are available.
/// </para>
/// </remarks>
/// <seealso cref="ILogSink"/>
public sealed class ConsoleLogSink : ILogSink
{
    public string Name => "Console";

    private readonly Dictionary<LogLevel, ConsoleColor> _colors = new()
    {
        { LogLevel.Trace, ConsoleColor.DarkGray },
        { LogLevel.Debug, ConsoleColor.Gray },
        { LogLevel.Info, ConsoleColor.White },
        { LogLevel.Warn, ConsoleColor.DarkYellow },
        { LogLevel.Error, ConsoleColor.Red },
        { LogLevel.Fatal, ConsoleColor.DarkRed },
    };

    public void Log(LogEntry entry)
    {
        Debug.Assert(_colors.TryGetValue(entry.Level, out ConsoleColor color), $"The color for log level '{entry.Level}' is not defined.");

        TextWriter standardOutput = entry.Level >= LogLevel.Error ? Console.Error : Console.Out;

        ConsoleColor tempColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        if (entry.Exception != null)
        {
            standardOutput.Write($"[{entry.TimeStamp:HH:mm:ss}][Thread-{entry.ThreadId}][{entry.Level}][{entry.Context}] {entry.Message} ");
            standardOutput.WriteLine($"{entry.Exception.GetType().Name}: {entry.Exception.Message}");
            standardOutput.WriteLine(entry.Exception.StackTrace ?? "No stack trace available");
        }
        else
            standardOutput.WriteLine($"[{entry.TimeStamp:HH:mm:ss}][Thread-{entry.ThreadId}][{entry.Level}][{entry.Context}] {entry.Message}");

        Console.ForegroundColor = tempColor;
    }
}