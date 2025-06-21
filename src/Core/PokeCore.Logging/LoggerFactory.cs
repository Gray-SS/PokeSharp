using PokeCore.DependencyInjection.Abstractions;

namespace PokeCore.Logging;

/// <summary>
/// Provides methods to create context-aware loggers.
/// </summary>
/// <remarks>
/// This factory is used by the DI container and framework internals to inject the appropriate <see cref="Logger"/>
/// into services or components based on their type.
/// </remarks>
public static class LoggerFactory
{
    /// <summary>
    /// Returns a logger instance associated with the given type.
    /// </summary>
    /// <param name="type">The type to associate with the logger context.</param>
    /// <returns>A new <see cref="Logger"/> instance with contextual awareness.</returns>
    public static Logger GetLogger(Type type)
    {
        LoggerSettings settings = ServiceLocator.GetRequiredService<LoggerSettings>();
        return new ContextLogger(settings, type.Name);
    }

    /// <summary>
    /// Returns a logger instance associated with the given type.
    /// </summary>
    /// <typeparam name="T">The type to associate with the logger context.</typeparam>
    /// <returns>A new <see cref="Logger"/> instance with contextual awareness.</returns>
    public static Logger GetLogger<T>() where T : class
    {
        LoggerSettings settings = ServiceLocator.GetRequiredService<LoggerSettings>();
        return new ContextLogger(settings, typeof(T).Name);
    }
}