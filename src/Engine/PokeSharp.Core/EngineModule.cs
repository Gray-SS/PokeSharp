using Ninject.Modules;
using PokeSharp.Core.Resolutions;
using PokeSharp.Core.Services;

namespace PokeSharp.Core;

public sealed class EngineModule : NinjectModule
{
    public override void Load()
    {
        Bind<Engine>().ToConstant(Engine.Instance);
        Bind<IReflectionManager>().To<ReflectionManager>().InSingletonScope();
        Bind<IResolutionManager>().To<ResolutionManager>().InSingletonScope();
        Bind<IEngineHookDispatcher>().To<EngineHookDispatcher>().InSingletonScope();
    }
}