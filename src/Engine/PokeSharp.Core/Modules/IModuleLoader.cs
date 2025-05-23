namespace PokeSharp.Core.Modules;

public interface IModuleLoader
{
    bool IsLoaded { get; }
    bool IsConfigured { get; }

    IReadOnlyCollection<IModule> LoadedModules { get; }

    void RegisterModule(IModule module);

    void LoadModules();
    void ConfigureModules();
}