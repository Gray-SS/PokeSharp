using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;
using PokeCore.IO.Services;

namespace PokeCore.IO.Extensions;

public static class DependencyInjection
{
    public static IServiceCollections AddPokeCoreIO(this IServiceCollections services)
    {
        services.AddSingleton<IVirtualFileSystem, VirtualFileSystem>();
        services.AddSingleton<IVirtualVolumeManager, VirtualVolumeManager>();

        return services;
    }
}