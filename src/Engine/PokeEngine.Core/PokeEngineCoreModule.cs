using PokeCore.DependencyInjection.Abstractions;
using PokeEngine.Core.Coroutines;
using PokeEngine.Core.Resolutions;
using PokeEngine.Core.Windowing;
using PokeEngine.Core.Modules;
using PokeCore.Hosting.Abstractions.Extensions;

namespace PokeEngine.Core;

public class PokeEngineCoreModule<TEngine> : EngineModule
    where TEngine : BaseEngine
{
    public override string Name => "PokéEngine Core";

    public override Version Version => new(1, 0, 0);

    public override void Configure(IServiceContainer services)
    {
    }

    public override void ConfigureServices(IServiceCollections services)
    {
        services.AddSingleton<TEngine>();
        services.AddSingleton<BaseEngine>(sp => sp.GetService<TEngine>());
        services.AddSingleton<IGameLoop>(sp => sp.GetService<TEngine>());
        services.AddHostedService<EngineHost<TEngine>>();

        // Define services for MonoGame
        services.AddSingleton(sp => sp.GetService<TEngine>().Window);
        services.AddSingleton(sp => sp.GetService<TEngine>().Graphics);
        services.AddSingleton(sp => sp.GetService<TEngine>().GraphicsDevice);
        services.AddSingleton(sp => sp.GetService<TEngine>().Content);

        // services.AddSingleton<IEngineHookDispatcher, EngineHookDispatcher>();
        services.AddSingleton<ICoroutineManager, CoroutineManager>();
        services.AddSingleton<IResolutionManager, ResolutionManager>();
        services.AddSingleton<IWindowManager, WindowManager>();
    }
}