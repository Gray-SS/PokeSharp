using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;
using PokeTools.ROM.Services;

namespace PokeTools.ROM.Extensions;

public static class DependencyInjection
{
    private static string GbaConfigPath => Path.Combine(
        AppContext.BaseDirectory,
        "configs",
        "gbas.yaml"
    );

    public static IServiceCollections AddPokeToolsRom(this IServiceCollections services)
    {
        services.AddSingleton<IGbaConfigProvider>(new GbaConfigProvider(GbaConfigPath));

        return services;
    }
}