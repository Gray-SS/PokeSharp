using Ninject;
using Ninject.Syntax;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Modules.Events;

namespace PokeSharp.Core.Modules;

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

    public virtual void Register(IModuleLoader loader)
    {
    }

    #endregion // Module API
}