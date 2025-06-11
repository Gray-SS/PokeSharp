using PokeCore.Common;
using PokeCore.DependencyInjection.Abstractions;

namespace PokeEngine.Core.Modules.Extensions;

public static class DIExtensions
{
    public static IServiceCollections AddPokeModule<TModule>(this IServiceCollections services) where TModule : EngineModule, new()
    {
        var module = new TModule();
        return services.AddPokeModule(module);
    }

    public static IServiceCollections AddPokeModule<TModule>(this IServiceCollections services, TModule module) where TModule : EngineModule
    {
        module.ConfigureServices(services);

        services.AddSingleton(module);
        services.AddSingleton<IEngineModule>(sp => sp.GetService<TModule>());

        return services;
    }

    public static IServiceContainer UsePokeModules(this IServiceContainer services)
    {
        IEnumerable<IEngineModule> modules = services.GetServices<IEngineModule>();

        foreach (IEngineModule module in modules)
            module.Configure(services);

        return services;
    }
}