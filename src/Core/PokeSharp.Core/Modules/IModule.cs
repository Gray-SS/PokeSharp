using Ninject;
using Ninject.Syntax;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Modules.Events;

namespace PokeSharp.Core.Modules;

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