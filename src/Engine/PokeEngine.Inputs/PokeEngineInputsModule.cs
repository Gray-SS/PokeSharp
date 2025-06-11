using PokeCore.DependencyInjection.Abstractions;
using PokeEngine.Core.Modules;

namespace PokeEngine.Inputs;

public sealed class PokeEngineInputsModule : EngineModule
{
    public override string Name => "PokéEngine Inputs";
    public override Version Version => new(1, 0, 0);

    public override void Configure(IServiceContainer services)
    {
    }

    public override void ConfigureServices(IServiceCollections services)
    {
        services.AddSingleton<IInputManager, InputManager>();
    }
}