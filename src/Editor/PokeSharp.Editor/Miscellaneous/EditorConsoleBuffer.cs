using PokeSharp.Core.Logging;

namespace PokeSharp.Editor.Miscellaneous;

public sealed class EditorConsoleBuffer : ILogOutput
{
    public string Name => "EditorConsole";

    public List<LogEntry> Entries => _entries;

    private readonly List<LogEntry> _entries = new();

    public void Clear()
    {
        _entries.Clear();
    }

    public void Log(in LogEntry entry)
    {
        _entries.Add(entry);
    }
}
