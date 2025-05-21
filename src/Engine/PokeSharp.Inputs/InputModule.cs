using Ninject.Modules;

namespace PokeSharp.Inputs;

public sealed class InputModule : NinjectModule
{
    public override void Load()
    {
        Bind<InputManager>().ToSelf().InSingletonScope();
    }
}