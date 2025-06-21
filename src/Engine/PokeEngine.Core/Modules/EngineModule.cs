using PokeCore.DependencyInjection.Abstractions;

namespace PokeEngine.Core.Modules;

public abstract class EngineModule : IEngineModule
{
    public abstract string Name { get; }
    public abstract Version Version { get; }

    public abstract void Configure(IServiceResolver services);
    public abstract void ConfigureServices(IServiceCollections services);
}