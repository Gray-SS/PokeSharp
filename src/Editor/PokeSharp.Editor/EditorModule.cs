using MonoGame.ImGuiNet;
using Ninject;
using Ninject.Syntax;
using PokeSharp.Assets.Services;
using PokeSharp.Core;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Modules;
using PokeSharp.Editor.ContentBrowser.Services;
using PokeSharp.Editor.Miscellaneous;
using PokeSharp.Editor.Services;

namespace PokeSharp.Editor;

public sealed class EditorModule : Module
{
    public override string Name => "Editor";

    public override void Configure(IKernel kernel)
    {
        kernel.Bind<EditorConsoleBuffer>().ToSelf().InSingletonScope();
        kernel.Bind<IProjectManager>().To<ProjectManager>().InSingletonScope();
        kernel.Bind<IGuiResourceManager>().To<GuiResourceManager>().InSingletonScope();
        kernel.Bind<EditorGuiRenderer>().ToSelf().InSingletonScope();
        kernel.Bind<IGuiHookDispatcher>().To<GuiHookDispatcher>().InSingletonScope();
        kernel.Bind<ISelectionManager>().To<SelectionManager>().InSingletonScope();
        kernel.Bind<IAssetMetadataStore>().To<LibraryMetadataStore>().InSingletonScope();

        kernel.Bind<IContentNavigator>().To<ContentNavigator>().InSingletonScope();
        kernel.Bind<IContentCacheService>().To<ContentCacheService>().InSingletonScope();
    }

    public override void ConfigureLogging(LoggerSettings settings, IResolutionRoot container)
    {
        EditorConsoleBuffer buffer = container.Get<EditorConsoleBuffer>();
        settings.AddOutput(buffer);
    }

    public override void Load()
    {
        IKernel kernel = App.Kernel;

        Engine engine = kernel.Get<Engine>();
        kernel.Bind<ImGuiRenderer>().ToConstant(new ImGuiRenderer(engine));
    }
}