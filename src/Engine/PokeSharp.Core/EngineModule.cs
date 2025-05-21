using Ninject.Modules;
using PokeSharp.Core.Services;

namespace PokeSharp.Core;

public sealed class EngineModule : NinjectModule
{
    public override void Load()
    {
        Bind<Engine>().ToConstant(Engine.Instance);
        Bind<ResolutionManager>().ToSelf().InSingletonScope();
        Bind<ReflectionManager>().ToSelf().InSingletonScope();
    }
}