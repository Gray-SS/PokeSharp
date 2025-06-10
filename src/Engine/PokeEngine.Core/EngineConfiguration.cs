using Ninject;
using PokeCore.Hosting.Logging;
using PokeCore.Hosting.Modules;

namespace PokeEngine.Core;

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