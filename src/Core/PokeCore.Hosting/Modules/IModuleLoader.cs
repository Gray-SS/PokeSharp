using PokeCore.Hosting.Modules.Events;

namespace PokeCore.Hosting.Modules;

public interface IModuleLoader
{
    bool IsLoaded { get; }
    bool IsConfigured { get; }

    IEnumerable<IModule> LoadedModules { get; }
    IEnumerable<IModule> RegisteredModules { get; }

    event EventHandler<ModuleStateChangedArgs>? ModuleStateChanged;

    void RegisterModule(IModule module);

    void LoadModules();
    void ConfigureModules();
}