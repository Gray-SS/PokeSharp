using PokeSharp.Core.Logging;

namespace PokeSharp.Editor.Miscellaneous;

public sealed class EditorConsoleBuffer : ILogOutput
{
    public string Name => "EditorConsole";

    public List<LogEntry> Entries => _entries;

    private readonly List<LogEntry> _entries = new();
    private readonly Queue<LogEntry> _queuedEntries = new();

    public void Clear()
    {
        _entries.Clear();
        _queuedEntries.Clear();
    }

    public void Log(in LogEntry entry)
    {
        _queuedEntries.Enqueue(entry);
        // _entries.Add(entry);
    }

    public void Update()
    {
        while (_queuedEntries.TryDequeue(out var entry))
            _entries.Add(entry);
    }
}
