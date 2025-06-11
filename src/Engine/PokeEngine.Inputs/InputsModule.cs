using Ninject;
using PokeCore.Hosting.Modules;

namespace PokeEngine.Inputs;

public sealed class InputsModule : Module
{
    public override string Name => "Inputs";

    public override void ConfigureServices(IKernel kernel)
    {
        kernel.Bind<IInputManager>().To<InputManager>().InSingletonScope();
    }
}