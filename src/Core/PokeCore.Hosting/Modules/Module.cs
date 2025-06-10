using Ninject;
using Ninject.Syntax;
using PokeCore.Hosting.Logging;
using PokeCore.Hosting.Modules.Events;

namespace PokeCore.Hosting.Modules;

public abstract class Module : IModule, IModuleInternal
{
    public IApp App => ServiceLocator.CurrentApp;
    public abstract string Name { get; }

    public ModuleState State
    {
        get => _state;
        set => SetState(value);
    }

    public event EventHandler<ModuleStateChangedArgs>? StateChanged;

    private ModuleState _state = ModuleState.NotInitialized;


    public void SetState(ModuleState state)
    {
        if (_state == state)
            return;

        ModuleState oldState = _state;
        _state = state;

        StateChanged?.Invoke(this, new ModuleStateChangedArgs(this, state, oldState));
    }


    #region Module API

    public abstract void Configure(IKernel kernel);

    public virtual void ConfigureLogging(LoggerSettings settings, IResolutionRoot container)
    {
    }

    public virtual void Load()
    {
    }

    public virtual void RegisterSubModules(IModuleLoader loader)
    {
    }

    #endregion // Module API
}