using Ninject;
using PokeSharp.Core;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Modules;

namespace PokeSharp.Editor;

public sealed class EditorEngine : Engine
{
    public EditorEngine(IKernel kernel, ILogger logger, IModuleLoader moduleLoader) : base(kernel, logger, moduleLoader)
    {
    }
}