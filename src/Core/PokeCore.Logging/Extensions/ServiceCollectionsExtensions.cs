using PokeCore.DependencyInjection.Abstractions;

namespace PokeCore.Logging.Extensions;

// TODO: Find a better way of handling logging configuration
public static class ServiceCollectionsExtensions
{
    public static IServiceCollections ConfigureLogging(this IServiceCollections services, Action<ILoggerConfiguration> configure)
    {
        var configBuilder = new LoggerConfiguration(services);
        configure.Invoke(configBuilder);

        return services;
    }
}