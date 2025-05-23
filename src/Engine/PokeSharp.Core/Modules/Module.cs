using Ninject;

namespace PokeSharp.Core.Modules;

public abstract class Module : IModule
{
    public IApp App => ServiceLocator.CurrentApp;
    public abstract string ModuleName { get; }

    public abstract void Configure(IKernel kernel);

    public virtual void Load()
    {
    }

    public virtual void RegisterSubmodules(IModuleLoader loader)
    {
    }
}