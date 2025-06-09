using System.Runtime.CompilerServices;

namespace PokeSharp.Core.Logging;

/// <summary>
/// A no-op implementation of <see cref="Logger"/> that ignores all log calls regardless of the outputs registered in the <see cref="LoggerSettings"/>.
/// </summary>
/// <remarks>
/// <para>
/// <b>When to use:</b>
/// Use <see cref="EmptyLogger"/> in scenarios where you want to disable logging entirely
/// (for example, in unit tests or in a performance‐critical build where even the minimal overhead of a logger is undesirable).
/// </para>
/// <para>
/// This implementation simply discards any message passed to it: none of the methods perform any action.
/// It can be registered instead of a real logger, ensuring that the rest of the code does not need to check for null or conditional flags.
/// </para>
/// </remarks>
public sealed class EmptyLogger : Logger
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Log(LogLevel level, string message, [CallerFilePath] string? callerFilePath = null, [CallerMemberName] string? callerMemberName = null)
    {
    }
}