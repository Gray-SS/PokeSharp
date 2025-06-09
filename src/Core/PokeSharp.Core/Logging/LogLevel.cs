namespace PokeSharp.Core.Logging;

/// <summary>
/// Represents the severity level of a log message.
/// </summary>
/// <remarks>
/// Log levels are used to filter and categorize messages based on their importance or criticality.
/// </remarks>
public enum LogLevel
{
    /// <summary>
    /// Fine-grained informational events, useful for diagnosing problems during development.
    /// </summary>
    Trace,

    /// <summary>
    /// General debugging information, less verbose than <see cref="Trace"/>.
    /// </summary>
    Debug,

    /// <summary>
    /// Informational messages that highlight the progress or state of the application.
    /// </summary>
    Info,

    /// <summary>
    /// Potentially harmful situations or warnings that do not stop the application.
    /// </summary>
    Warn,

    /// <summary>
    /// Error events that may allow the application to continue running.
    /// </summary>
    Error,

    /// <summary>
    /// Critical errors indicating the application is likely to crash or be in an unstable state.
    /// </summary>
    Fatal,
}