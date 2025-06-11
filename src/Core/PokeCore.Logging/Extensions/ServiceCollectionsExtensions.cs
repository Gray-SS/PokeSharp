using PokeCore.DependencyInjection.Abstractions;

namespace PokeCore.Logging.Extensions;

public static class ServiceCollectionsExtensions
{
    public static IServiceCollections ConfigureLogging(this IServiceCollections services, Action<ILoggerConfiguration> configure)
    {
        var configBuilder = new LoggerConfiguration(services);
        configure.Invoke(configBuilder);

        return services;
    }
}