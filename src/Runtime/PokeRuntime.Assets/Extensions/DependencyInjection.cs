using PokeRuntime.Assets.Loaders;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeRuntime.Assets.Extensions;

public static class DependencyInjection
{
    public static IServiceCollections AddPokeRuntimeAssets(this IServiceCollections services)
    {
        services.AddSingleton<IAssetManager, RuntimeAssetManager>();
        services.AddTransient<IRuntimeAssetLoader, RuntimeTextureLoader>();
        services.AddTransient<IRuntimeAssetLoader, RuntimeSpriteLoader>();

        return services;
    }
}