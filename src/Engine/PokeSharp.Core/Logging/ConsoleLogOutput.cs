using System.Drawing;

namespace PokeSharp.Core.Logging;

public sealed class ConsoleLogOutput : ILogOutput
{
    public string Name => "Console";

    private readonly Dictionary<LogLevel, ConsoleColor> _colors = new()
    {
        { LogLevel.Debug, ConsoleColor.DarkGray },
        { LogLevel.Info, ConsoleColor.Gray },
        { LogLevel.Warn, ConsoleColor.DarkYellow },
        { LogLevel.Error, ConsoleColor.Red },
        { LogLevel.Fatal, ConsoleColor.DarkRed },
    };

    public void Log(in LogEntry entry)
    {
        if (!_colors.TryGetValue(entry.Level, out ConsoleColor color))
            color = ConsoleColor.Magenta;

        ConsoleColor tempColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        System.Console.WriteLine($"[{entry.TimeStamp:yyyy:MM:dd}] [{entry.Context}::{entry.Level}] {entry.Message}");
        Console.ForegroundColor = tempColor;
    }
}