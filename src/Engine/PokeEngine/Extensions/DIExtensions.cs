using PokeEngine.Core;

using PokeCore.DependencyInjection.Abstractions;
using PokeEngine.Core.Modules.Extensions;

namespace PokeEngine.Extensions;

public static class DIExtensions
{
    public static IServiceCollections AddPokeEngineEssentials<TEngine>(this IServiceCollections services)
        where TEngine : BaseEngine
    {
        services.AddPokeModule<PokeEngineEssentials<TEngine>>();
        return services;
    }
}