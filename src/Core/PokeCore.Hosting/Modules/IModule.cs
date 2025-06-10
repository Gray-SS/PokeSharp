using Ninject;
using Ninject.Syntax;
using PokeCore.Hosting.Logging;
using PokeCore.Hosting.Modules.Events;

namespace PokeCore.Hosting.Modules;

internal interface IModuleInternal : IModule
{
    new ModuleState State { get; set; }

    void SetState(ModuleState state);
}

public interface IModule
{
    IApp App { get; }
    string Name { get; }

    ModuleState State { get; }

    event EventHandler<ModuleStateChangedArgs>? StateChanged;

    void Load();

    // TODO: Replace the IKernel by IBindingRoot to only have the ability to bind
    void Configure(IKernel kernel);

    void RegisterSubModules(IModuleLoader loader);

    void ConfigureLogging(LoggerSettings settings, IResolutionRoot container);
}