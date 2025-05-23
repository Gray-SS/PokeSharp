using Ninject;
using PokeSharp.Core.Modules;

namespace PokeSharp.Inputs;

public sealed class InputsModule : Module
{
    public override string ModuleName => "Inputs";

    public override void Configure(IKernel kernel)
    {
        kernel.Bind<IInputManager>().To<InputManager>().InSingletonScope();
    }
}