using PokeRuntime.Assets.Loaders;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeRuntime.Assets.Extensions;

public static class DependencyInjection
{
    public static IServiceCollections AddPokeRuntimeAssets(this IServiceCollections services)
    {
        services.AddTransient<IAssetManager, RuntimeAssetManager>();
        services.AddTransient<IRuntimeAssetLoader<RuntimeTexture>, RuntimeTextureLoader>();
        services.AddTransient<IRuntimeAssetLoader<RuntimeSprite>, RuntimeSpriteLoader>();

        return services;
    }
}