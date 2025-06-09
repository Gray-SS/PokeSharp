using MonoGame.ImGuiNet;
using Ninject;
using Ninject.Syntax;
using PokeSharp.Assets.Services;
using PokeSharp.Core;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Logging.Outputs;
using PokeSharp.Core.Modules;
using PokeSharp.Editor.ContentBrowser.Services;
using PokeSharp.Editor.Services;
using PokeSharp.Engine.Core;

namespace PokeSharp.Editor;

public sealed class EditorModule : Module
{
    public override string Name => "Editor";

    public override void Configure(IKernel kernel)
    {
        kernel.Bind<MemoryLogSink>().ToSelf().InSingletonScope();
        kernel.Bind<IProjectManager>().To<ProjectManager>().InSingletonScope();
        kernel.Bind<IGuiResourceManager>().To<GuiResourceManager>().InSingletonScope();
        kernel.Bind<EditorGuiRenderer>().ToSelf().InSingletonScope();
        kernel.Bind<IEditorViewManager>().To<EditorViewManager>().InSingletonScope();
        kernel.Bind<ISelectionManager>().To<SelectionManager>().InSingletonScope();
        kernel.Bind<IAssetMetadataStore>().To<LibraryMetadataStore>().InSingletonScope();

        kernel.Bind<IContentNavigator>().To<ContentNavigator>().InSingletonScope();
        kernel.Bind<IContentCacheService>().To<ContentCacheService>().InSingletonScope();
    }

    public override void ConfigureLogging(LoggerSettings settings, IResolutionRoot container)
    {
        MemoryLogSink memorySink = container.Get<MemoryLogSink>();
        settings.AddOutput(memorySink);
    }

    public override void Load()
    {
        IKernel kernel = App.Kernel;

        BaseEngine engine = kernel.Get<BaseEngine>();
        kernel.Bind<ImGuiRenderer>().ToConstant(new ImGuiRenderer(engine));
    }
}