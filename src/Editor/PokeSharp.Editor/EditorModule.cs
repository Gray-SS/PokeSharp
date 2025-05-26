using Ninject;
using Ninject.Syntax;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Modules;
using PokeSharp.Editor.Miscellaneous;
using PokeSharp.Editor.Services;

namespace PokeSharp.Editor;

public sealed class EditorModule : Module
{
    public override string Name => "Editor";

    public override void Configure(IKernel kernel)
    {
        kernel.Bind<EditorConsoleBuffer>().ToSelf().InSingletonScope();
        kernel.Bind<IEditorProjectManager>().To<EditorProjectManager>().InSingletonScope();
        kernel.Bind<EditorGuiRenderer>().ToSelf().InSingletonScope();
        kernel.Bind<IGuiHookDispatcher>().To<GuiHookDispatcher>().InSingletonScope();
    }

    public override void ConfigureLogging(LoggerSettings settings, IResolutionRoot container)
    {
        EditorConsoleBuffer buffer = container.Get<EditorConsoleBuffer>();
        settings.AddOutput(buffer);
    }
}
