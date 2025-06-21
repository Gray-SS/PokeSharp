using PokeCore.DependencyInjection.Abstractions;
using PokeEngine.Core.Coroutines;
using PokeEngine.Core.Resolutions;
using PokeEngine.Core.Windowing;
using PokeEngine.Core.Modules;
using PokeCore.Hosting.Abstractions.Extensions;
using Microsoft.Xna.Framework;
using PokeEngine.Core.Timing;
using PokeEngine.Core.Modules.Extensions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeEngine.Core;

public class PokeEngineCoreModule<TEngine> : EngineModule
    where TEngine : BaseEngine
{
    public override string Name => "PokéEngine Core";

    public override Version Version => new(1, 0, 0);

    public override void Configure(IServiceResolver services)
    {
        services.UsePokeModules();
    }

    public override void ConfigureServices(IServiceCollections services)
    {
        services.AddSingleton<TEngine>();
        services.AddSingleton<Game>(sp => sp.GetRequiredService<TEngine>());
        services.AddSingleton<BaseEngine>(sp => sp.GetRequiredService<TEngine>());
        services.AddSingleton<IGameLoop>(sp => sp.GetRequiredService<TEngine>());

        // Define services for MonoGame
        services.AddSingleton(sp => sp.GetRequiredService<TEngine>().Window);
        services.AddSingleton(sp => sp.GetRequiredService<TEngine>().Graphics);
        services.AddSingleton(sp => sp.GetRequiredService<TEngine>().GraphicsDevice);
        services.AddSingleton(sp => sp.GetRequiredService<TEngine>().Content);

        services.AddSingleton<ICoroutineManager, CoroutineManager>();
        services.AddSingleton<IResolutionManager, ResolutionManager>();
        services.AddSingleton<IWindowManager, WindowManager>();
        services.AddSingleton<ITimingService, TimingService>();

        services.AddHostedService<EngineHost>();
    }
}