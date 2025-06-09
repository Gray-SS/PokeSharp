namespace PokeSharp.Core.Logging;

/// <summary>
/// Represents a <b>target</b> or <b>destination</b> for log messages (<c>e.g.</c> <b>console</b>, <b>file</b>, <b>network</b>).
/// Implementations of this interface are responsible for handling and persisting log entries.
/// </summary>
/// <remarks>
/// <b>Global log sinks:</b> To configure global sinks for your application, override the virtual <see cref="App.ConfigureLogging(LoggerSettings)"/> method.
/// <br/><br/>
/// <b>Module-specific log sinks:</b> For module-specific sinks (<c>e.g.</c> the EditorModule uses <see cref="Outputs.MemoryLogSink"/>),
/// override <see cref="Modules.Module.ConfigureLogging(LoggerSettings, Ninject.Syntax.IResolutionRoot)"/>.
/// <br/><br/>
/// <i>Log sinks can also be added or removed dynamically by injecting <see cref="LoggerSettings"/> into your services.</i>
/// </remarks>
public interface ILogSink
{
    /// <summary>
    /// Gets the name of the output sink.
    /// This can be used for identification, diagnostics, or configuration purposes.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Writes the given log entry to the output.
    /// Implementations must handle the formatting, filtering, and delivery as appropriate.
    /// </summary>
    /// <param name="entry">The log entry to write.</param>
    void Log(LogEntry entry);
}