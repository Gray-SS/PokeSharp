using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PokeCore.Hosting.Logging;

/// <summary>
/// Defines a logging interface with support for multiple log levels and automatic inclusion of caller context information.
/// </summary>
/// <remarks>
/// This interface provides a standardized way to log messages at different severity levels (e.g., <c>Trace</c>, <c>Debug</c>, <c>Info</c>, etc.).
/// Caller metadata (such as file path and member name) is captured automatically for contextual logging.
/// <br/><br/>
/// The default implementation is <see cref="ContextLogger"/>, which binds log messages to a specific type context
/// and is typically injected using <see cref="LoggerFactory.GetLogger(Type)"/>.
/// </remarks>
public abstract class Logger
{
    /// <summary>
    /// Logs a message at the specified log level.
    /// </summary>
    /// <param name="level">The severity level of the log message.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="callerFilePath">The full path of the source file that initiated the log call. Automatically provided by the compiler.</param>
    /// <param name="callerMemberName">The name of the method or property that initiated the log call. Automatically provided by the compiler.</param>
    /// <remarks>
    /// <b>Note:</b> Do not call this method directly for <c>Debug</c> or <c>Trace</c> messages,
    /// because the compiler cannot discard those calls in release builds.
    /// Instead, use the level-specific methods <see cref="Trace"/>, <see cref="Debug"/>, <see cref="Info"/>, <see cref="Warn"/>, etc.
    /// to allow proper conditional compilation.
    /// </remarks>
    public abstract void Log(LogLevel level,
                             string message,
                             [CallerFilePath] string? callerFilePath = null,
                             [CallerMemberName] string? callerMemberName = null);

    /// <summary>
    /// Logs a trace-level message. Typically used for extremely detailed and low-level diagnostic output.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="callerFilePath">The source file that generated the log call.</param>
    /// <param name="callerMemberName">The name of the calling method.</param>
    /// <remarks>
    /// This call will be completely removed from release builds if the <c>TRACE</c> symbol is not defined,
    /// regardless of the <see cref="LogLevel"/> configured in <see cref="LoggerSettings"/>.
    /// </remarks>
    [Conditional("TRACE")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Trace(string message,
                      [CallerFilePath] string? callerFilePath = null,
                      [CallerMemberName] string? callerMemberName = null)
    {
        Log(LogLevel.Trace, message, callerFilePath, callerMemberName);
    }

    /// <summary>
    /// Logs a debug-level message. Used for general debugging information useful during development.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="callerFilePath">The source file that generated the log call.</param>
    /// <param name="callerMemberName">The name of the calling method.</param>
    /// <remarks>
    /// This call will be completely removed from release builds,
    /// regardless of the <see cref="LogLevel"/> configured in <see cref="LoggerSettings"/>.
    /// </remarks>
    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Debug(string message,
                     [CallerFilePath] string? callerFilePath = null,
                     [CallerMemberName] string? callerMemberName = null)
    {
        Log(LogLevel.Debug, message, callerFilePath, callerMemberName);
    }

    /// <summary>
    /// Logs an informational message. Used for general, high-level runtime events.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="callerFilePath">The source file that generated the log call.</param>
    /// <param name="callerMemberName">The name of the calling method.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Info(string message,
                     [CallerFilePath] string? callerFilePath = null,
                     [CallerMemberName] string? callerMemberName = null)
    {
        Log(LogLevel.Info, message, callerFilePath, callerMemberName);
    }

    /// <summary>
    /// Logs a warning message. Indicates a potential issue that does not interrupt program execution.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="callerFilePath">The source file that generated the log call.</param>
    /// <param name="callerMemberName">The name of the calling method.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Warn(string message,
                     [CallerFilePath] string? callerFilePath = null,
                     [CallerMemberName] string? callerMemberName = null)
    {
        Log(LogLevel.Warn, message, callerFilePath, callerMemberName);
    }

    /// <summary>
    /// Logs an error message. Indicates a problem that has occurred but is not fatal.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="callerFilePath">The source file that generated the log call.</param>
    /// <param name="callerMemberName">The name of the calling method.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Error(string message,
                      [CallerFilePath] string? callerFilePath = null,
                      [CallerMemberName] string? callerMemberName = null)
    {
        Log(LogLevel.Error, message, callerFilePath, callerMemberName);
    }

    /// <summary>
    /// Logs a fatal error message. Indicates a critical failure that will likely lead to termination.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="callerFilePath">The source file that generated the log call.</param>
    /// <param name="callerMemberName">The name of the calling method.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fatal(string message,
               [CallerFilePath] string? callerFilePath = null,
               [CallerMemberName] string? callerMemberName = null)
    {
        Log(LogLevel.Fatal, message, callerFilePath, callerMemberName);
    }
}
