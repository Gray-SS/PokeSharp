using System.Runtime.CompilerServices;

namespace PokeCore.Logging;

/// <summary>
/// Provides a logger that is bound to a specific context (typically a <see cref="Type"/>), enabling more granular and structured logging.
/// </summary>
/// <remarks>
/// <b>Note:</b> This is the default implementation of <see cref="Logger"/>, and is typically injected into your services via <see cref="LoggerFactory.GetLogger(Type)"/>.
/// <br/>
/// If dependency injection is not available in your context, you can manually obtain a logger instance using <see cref="LoggerFactory.GetLogger(Type)"/>.
/// </remarks>
public sealed class ContextLogger : Logger
{
    private static readonly Lock _lock = new();

    private readonly string _context;
    private readonly LoggerSettings _settings;

    public ContextLogger(LoggerSettings settings, string context)
    {
        _context = context;
        _settings = settings;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Log(LogLevel level,
                             string message,
                             Exception? exception = null,
                             [CallerFilePath] string? callerFilePath = null,
                             [CallerMemberName] string? callerMemberName = null)
    {
        if (level < _settings.LogLevel)
            return;

        var timestamp = DateTime.Now;
        var currentThreadId = Environment.CurrentManagedThreadId;
        var entry = new LogEntry(timestamp, level, _context, message, currentThreadId, exception, callerFilePath, callerMemberName);

        _lock.Enter();
        foreach (ILogSink output in _settings.Outputs)
        {
            output.Log(entry);
        }
        _lock.Exit();
    }
}

/// <summary>
/// Provides a logger that is bound to a specific context (typically a <see cref="Type"/>), enabling more granular and structured logging.
/// </summary>
/// <remarks>
/// <b>Note:</b> This is the default implementation of <see cref="Logger"/>, and is typically injected into your services via <see cref="LoggerFactory.GetLogger(Type)"/>.
/// <br/>
/// If dependency injection is not available in your context, you can manually obtain a logger instance using <see cref="LoggerFactory.GetLogger(Type)"/>.
/// </remarks>
public sealed class ContextLogger<T> : Logger<T>
{
    private readonly ContextLogger _inner;

    public ContextLogger(LoggerSettings settings)
    {
        _inner = new ContextLogger(settings, typeof(T).Name ?? "Unknown");
    }

    public override void Log(LogLevel level, string message, Exception? exception, [CallerFilePath] string? callerFilePath = null, [CallerMemberName] string? callerMemberName = null)
    {
        _inner.Log(level, message, exception, callerFilePath, callerMemberName);
    }
}