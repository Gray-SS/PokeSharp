using PokeCore.Diagnostics;

namespace PokeCore.Logging;

/// <summary>
/// Represents a single structured log message, including metadata such as timestamp, level, and source context.
/// </summary>
/// <remarks>
/// Log entries are passed to <see cref="ILogSink"/> implementations, which decide how to store or display them.
/// </remarks>
public class LogEntry
{
    /// <summary>
    /// The timestamp when the log message was generated.
    /// </summary>
    public readonly DateTime TimeStamp;

    /// <summary>
    /// The severity level of the log message.
    /// </summary>
    public readonly LogLevel Level;

    /// <summary>
    /// The logical context or category associated with this log entry (e.g., class name, subsystem).
    /// </summary>
    public readonly string Context;

    /// <summary>
    /// The content of the log message.
    /// </summary>
    public readonly string Message;

    /// <summary>
    /// The file path of the calling source code, if available.
    /// </summary>
    public readonly string? CallerFilePath;

    /// <summary>
    /// The name of the method or member that produced the log message, if available.
    /// </summary>
    public readonly string? CallerMemberName;

    /// <summary>
    /// The thrown exception, if available.
    /// </summary>
    public readonly Exception? Exception;

    /// <summary>
    /// The id of the thread that executed the log
    /// </summary>
    public readonly int ThreadId;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntry"/> class with full metadata.
    /// </summary>
    /// <param name="timeStamp">The creation time of the log message.</param>
    /// <param name="level">The log level of the message.</param>
    /// <param name="context">The context in which the message was generated.</param>
    /// <param name="message">The actual log message content.</param>
    /// <param name="callerFilePath">Optional file path of the caller.</param>
    /// <param name="callerMemberName">Optional name of the caller member.</param>
    public LogEntry(
        DateTime timeStamp,
        LogLevel level,
        string context,
        string message,
        int threadId,
        Exception? exception,
        string? callerFilePath = null,
        string? callerMemberName = null)
    {
        ThrowHelper.Assert(timeStamp <= DateTime.Now.AddSeconds(1), $"The log entry time stamp can not be in the future. TimeStamp: {timeStamp}, Now: {DateTime.Now}");
        ThrowHelper.Assert(timeStamp > DateTime.MinValue, $"The log entry time stamp must be greater than the min value. TimeStamp: {timeStamp}, MinValue: {DateTime.MinValue}");
        ThrowHelper.AssertNotNullOrWhitespace(context, "The log entry context must be not null or whitespace");
        ThrowHelper.AssertNotNullOrWhitespace(message, "The log entry message must be not null or whitespace");

        TimeStamp = timeStamp;
        Exception = exception;
        Level = level;
        Context = context;
        Message = message;
        ThreadId = threadId;
        CallerFilePath = callerFilePath;
        CallerMemberName = callerMemberName;
    }
}