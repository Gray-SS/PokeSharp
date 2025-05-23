using Ninject;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Modules;
using PokeSharp.Editor.UI;
using PokeSharp.Editor.Views;

namespace PokeSharp.Editor;

public sealed class EditorModule : Module
{
    public override string ModuleName => "Editor";

    public override void Configure(IKernel kernel)
    {
    }

    public override void Load()
    {
        IKernel kernel = App.Kernel;

        DebugViewer debugViewer = kernel.Get<DebugViewer>();
        LoggerSettings settings = kernel.Get<LoggerSettings>();
        settings.Outputs.Add(debugViewer);
    }

    public override void RegisterSubmodules(IModuleLoader loader)
    {
        loader.RegisterModule(new EditorUiModule());
    }
}
