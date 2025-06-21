using PokeEngine.Core;
using PokeEngine.Core.Modules.Extensions;
using PokeCore.DependencyInjection.Abstractions;

namespace PokeEngine.Extensions;

public static class DependencyInjection
{
    public static IServiceResolver UsePokeEngineEssentials(this IServiceResolver services)
    {
        services.UsePokeModules();
        return services;
    }

    public static IServiceCollections AddPokeEngineEssentials<TEngine>(this IServiceCollections services)
        where TEngine : BaseEngine
    {
        services.AddPokeModule<PokeEngineEssentials<TEngine>>();
        return services;
    }
}