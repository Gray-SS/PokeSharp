using Ninject;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Modules;

namespace PokeSharp.Engine.Core;

public sealed class EngineConfiguration
{
    public IKernel Kernel { get; }
    public IModuleLoader ModuleLoader { get; }

    public EngineConfiguration(IKernel kernel, IModuleLoader moduleLoader)
    {
        Kernel = kernel;
        ModuleLoader = moduleLoader;
    }
}