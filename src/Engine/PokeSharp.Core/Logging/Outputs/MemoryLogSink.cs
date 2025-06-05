using System.Collections.Concurrent;

namespace PokeSharp.Core.Logging.Outputs;

/// <summary>
/// Keeps all log entries in memory for the lifetime of this sink instance,
/// and exposes them for external viewers (e.g., an ImGui window).
/// </summary>
/// <remarks>
/// This sink is intended for in‐engine debugging or editor UIs where you want to display
/// every log message in a scrollable view. In PokeSharp's editor, <see cref="OutputView"/>
/// reads from <see cref="CollectedEntries"/> each frame to populate an ImGui window.
/// <br/><br/>
/// Be aware that if many entries accumulate, iterating and rendering them each frame
/// can increase memory usage and cause visible lag.
/// </remarks>
public sealed class MemoryLogSink : ILogSink
{
    public string Name => "Memory";

    /// <summary>
    /// Gets a snapshot of the currently collected entries
    /// </summary>
    public IReadOnlyCollection<LogEntry> CollectedEntries => _entries;

    /// <summary>
    /// Triggered when a new log entry is collected
    /// </summary>
    public event EventHandler<MemoryLogSinkEventArgs>? OnEntryCollected;

    private readonly List<LogEntry> _entries = new();
    private readonly ConcurrentQueue<LogEntry> _queuedEntries = new();

    public void Clear()
    {
        _entries.Clear();
        _queuedEntries.Clear();
    }

    public void Log(LogEntry entry)
    {
        _queuedEntries.Enqueue(entry);
    }

    public void Update()
    {
        while (_queuedEntries.TryDequeue(out LogEntry? entry))
        {
            _entries.Add(entry);
            OnEntryCollected?.Invoke(this, new MemoryLogSinkEventArgs(this, entry));
        }
    }
}

public sealed class MemoryLogSinkEventArgs : EventArgs
{
    public LogEntry LogEntry { get; }
    public MemoryLogSink Sink { get; }

    public MemoryLogSinkEventArgs(MemoryLogSink sink, LogEntry logEntry)
    {
        LogEntry = logEntry;
        Sink = sink;
    }
}