using PokeCore.DependencyInjection.Abstractions;

namespace PokeCore.Hosting.Abstractions.Extensions;

public static class ServiceCollectionsExtensions
{
    public static IServiceCollections AddHostedService<THosted>(this IServiceCollections services)
        where THosted : class, IHostedService
    {
        services.AddSingleton<THosted>();
        services.AddSingleton<IHostedService>(sc => sc.GetService<THosted>());

        return services;
    }
}