using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PokeSharp.Core;

/// <summary>
/// Provides helper methods for throwing exceptions and asserting conditions,
/// useful for centralizing argument validation logic.
/// </summary>
public static class ThrowHelper
{
    /// <summary>
    /// Default message used when no custom message is specified during assertions.
    /// </summary>
    private const string DefaultMessage = "This should never happen if the code is functioning correctly";

    /// <summary>
    /// Asserts that a reference is not null during debug builds,
    /// and throws an <see cref="ArgumentNullException"/> in release builds if it is null.
    /// </summary>
    /// <typeparam name="T">The type of the value to check. Must be a reference type.</typeparam>
    /// <param name="value">The value to validate against null.</param>
    /// <param name="message">Optional custom message. If null, a default message will be used.</param>
    /// <param name="paramName">
    /// The name of the parameter being checked. Automatically inferred using <see cref="CallerArgumentExpressionAttribute"/>.
    /// </param>
    /// <param name="callerName">
    /// The name of the member calling the function. Automatically inferred using <see cref="CallerMemberNameAttribute"/>.
    /// </param>
    /// <param name="callerFilePath">
    /// The full path of the source file containing the caller. Automatically inferred using <see cref="CallerFilePathAttribute"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AssertNotNull<T>(
                T value,
                string? message = null,
                [CallerArgumentExpression(nameof(value))] string? paramName = null,
                [CallerMemberName] string? callerName = null,
                [CallerFilePath] string? callerFilePath = null)
                where T : class
    {
        Debug.Assert(value != null, $"""
            [Null Assertion Failed]
            => File: {callerFilePath}
            => Method/property name: {callerName}
            => Parameter: '{paramName}' was null.
            Message: {message ?? DefaultMessage}.
        """);

        if (value is null)
            throw new ArgumentNullException(paramName, message ?? $"The parameter '{paramName}' cannot be null.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AssertNotNullOrWhitespace(
        string value,
        string? message = null,
        [CallerArgumentExpression(nameof(value))] string? paramName = null,
        [CallerMemberName] string? callerName = null,
        [CallerFilePath] string? callerFilePath = null)
    {
        Debug.Assert(!string.IsNullOrWhiteSpace(value), $"""
            [NullOrEmpty Assertion Failed]
            => File: {callerFilePath}
            => Method/property name: {callerName}
            => Parameter: '{paramName}' was null.
            Message: {message ?? DefaultMessage}.
        """);

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(paramName, message ?? $"The parameter '{paramName}' cannot be null or empty.");
    }

    /// <summary>
    /// Asserts a condition in debug builds, and throws an <see cref="ArgumentNullException"/>
    /// in release builds if the condition is <c>false</c>.
    /// </summary>
    /// <param name="condition">The condition to assert and validate.</param>
    /// <param name="message">Optional custom message. If null, a default message will be used.</param>
    /// <param name="paramName">
    /// The name of the parameter being checked. Automatically inferred using <see cref="CallerArgumentExpressionAttribute"/>.
    /// </param>
    /// <param name="callerName">
    /// The name of the member calling the function. Automatically inferred using <see cref="CallerMemberNameAttribute"/>.
    /// </param>
    /// <param name="callerFilePath">
    /// The full path of the source file containing the caller. Automatically inferred using <see cref="CallerFilePathAttribute"/>.
    /// </param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="condition"/> is <c>false</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert(
                bool condition,
                string? message = null,
                [CallerArgumentExpression(nameof(condition))] string? conditionExpr = null,
                [CallerMemberName] string? callerName = null,
                [CallerFilePath] string? callerFilePath = null)
    {
        Debug.Assert(condition, $"""
            [Assertion Failed]
            => File: {callerFilePath}
            => Method/property name: {callerName}
            => Failed condition: {conditionExpr}.
            Message: {message ?? DefaultMessage}.
        """);

        if (!condition)
            throw new ArgumentException(message ?? $"Assertion failed: {conditionExpr}. {message ?? DefaultMessage}");
    }
}