using Ninject.Modules;
using PokeSharp.Core.Resolutions;
using PokeSharp.Core.Services;

namespace PokeSharp.Core;

public class CoreModule : NinjectModule
{
    public override void Load()
    {
        Bind<Engine>().ToConstant(Engine.Instance);
        Bind<IReflectionManager>().To<ReflectionManager>().InSingletonScope();
        Bind<IResolutionManager>().To<ResolutionManager>().InSingletonScope();
        Bind<IEngineHookDispatcher>().To<EngineHookDispatcher>().InSingletonScope();
    }
}

public sealed class CoreModule<TEngine> : CoreModule
    where TEngine : Engine
{
    public override void Load()
    {
        base.Load();

        Bind<TEngine>().ToConstant((TEngine)Engine.Instance);
    }
}