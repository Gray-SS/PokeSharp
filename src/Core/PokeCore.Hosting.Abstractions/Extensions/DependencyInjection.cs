using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeCore.Hosting.Abstractions.Extensions;

public static class DependencyInjection
{
    public static IServiceCollections AddHostedService<THosted>(this IServiceCollections services)
        where THosted : class, IHostedService
    {
        services.AddSingleton<THosted>();
        services.AddSingleton<IHostedService>(sc => sc.GetRequiredService<THosted>());

        return services;
    }
}