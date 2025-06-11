using Ninject;
using PokeCore.Hosting;
using PokeLab.Application;
using PokeLab.Infrastructure;
using PokeLab.Presentation;
using PokeLab.Presentation.ImGui;

namespace PokeLab.Host;

public sealed class EditorApp : App
{
    public override string AppName => "PokéSharp Editor";
    public override Version AppVersion => new(1, 0, 0);

    protected override void ConfigureModules(IModuleLoader loader)
    {
        loader.RegisterModule(new PokeLabApplicationModule());
        loader.RegisterModule(new PokeLabInfrastructureModule());
        loader.RegisterModule(new PokeLabPresentationImGuiModule());
    }

    protected override void ConfigureServices(IKernel kernel)
    {
    }

    protected override void ConfigureLogging(LoggerSettings settings)
    {
        settings.AddOutput(new ConsoleLogSink());
    }

    protected override void OnRun()
    {
        ITickSource source = Kernel.Get<ITickSource>();
        source.Run();
    }
}