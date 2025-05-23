using Ninject;

namespace PokeSharp.Core.Modules;

public interface IModule
{
    IApp App { get; }
    string ModuleName { get; }

    void Load();
    void Configure(IKernel kernel);
    void RegisterSubmodules(IModuleLoader loader);
}