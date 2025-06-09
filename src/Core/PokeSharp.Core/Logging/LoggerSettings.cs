namespace PokeSharp.Core.Logging;

/// <summary>
/// Represents the global configuration for the application's logging system,
/// including log level filtering and registered log sinks.
/// </summary>
/// <remarks>
/// This class is typically used to configure the logger behavior at application startup,
/// such as setting the minimum log level and managing which log sinks are active (e.g., console, file, memory).
/// <br/><br/>
/// You can inject this service to dynamically add or remove outputs at runtime, or to adjust verbosity.
/// </remarks>
public sealed class LoggerSettings
{
    /// <summary>
    /// Gets or sets the minimum <see cref="LogLevel"/> for log messages to be processed.
    /// Messages below this threshold will be ignored.
    /// </summary>
    public LogLevel LogLevel { get; set; }

    /// <summary>
    /// Gets the list of active <see cref="ILogSink"/> instances that will receive log messages.
    /// </summary>
    public IReadOnlyCollection<ILogSink> Outputs => _outputs;

    private readonly HashSet<ILogSink> _outputs;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerSettings"/> class with default settings.
    /// </summary>
    /// <remarks>
    /// The default <see cref="LogLevel"/> is <see cref="LogLevel.Debug"/>.
    /// </remarks>
    public LoggerSettings()
    {
        LogLevel = LogLevel.Debug;
        _outputs = new HashSet<ILogSink>();
    }

    /// <summary>
    /// Sets the minimum log level for message filtering.
    /// </summary>
    /// <param name="level">The new log level threshold.</param>
    public void SetLogLevel(LogLevel level)
    {
        LogLevel = level;
    }

    /// <summary>
    /// Removes all registered log sinks.
    /// </summary>
    public void ClearOutputs()
    {
        _outputs.Clear();
    }

    /// <summary>
    /// Adds a new log sink to the list of outputs.
    /// </summary>
    /// <param name="output">The log sink to add.</param>
    public void AddOutput(ILogSink output)
    {
        if (_outputs.Contains(output))
            return;

        _outputs.Add(output);
    }

    /// <summary>
    /// Removes a log sink from the list of outputs.
    /// </summary>
    /// <param name="output">The log sink to remove.</param>
    public void RemoveOutput(ILogSink output)
    {
        _outputs.Remove(output);
    }
}