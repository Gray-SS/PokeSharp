namespace PokeSharp.Core.Logging;

public static class LoggerFactory
{
    public static ILogger GetLogger(Type type)
    {
        LoggerSettings settings = ServiceLocator.GetService<LoggerSettings>();
        return new ContextedLogger(settings, type.Name);
    }
}