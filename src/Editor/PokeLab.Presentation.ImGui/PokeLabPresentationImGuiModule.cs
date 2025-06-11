using Ninject;
using PokeCore.Hosting.Modules;
using PokeEngine;

namespace PokeLab.Presentation.ImGui;

public sealed class PokeLabPresentationImGuiModule : Module
{
    public override string Name => "PokeLab - ImGui Implementation";

    public override void ConfigureServices(IKernel kernel)
    {
        kernel.Bind<ITickSource>().To<ImGuiTickSource>();
    }

    public override void RegisterSubModules(IModuleLoader loader)
    {
        loader.RegisterModule(new PokeEngineEssentials());
    }
}