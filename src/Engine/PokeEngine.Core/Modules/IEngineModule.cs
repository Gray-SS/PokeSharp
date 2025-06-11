using PokeCore.DependencyInjection.Abstractions;

namespace PokeEngine.Core.Modules;

public interface IEngineModule
{
    string Name { get; }

    void Configure(IServiceContainer services);
    void ConfigureServices(IServiceCollections services);
}