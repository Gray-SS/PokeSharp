using MonoGame.ImGuiNet;
using Ninject;
using Ninject.Syntax;
using PokeEngine.Assets.Services;
using PokeCore.Hosting;
using PokeCore.Hosting.Logging;
using PokeCore.Hosting.Logging.Outputs;
using PokeCore.Hosting.Modules;
using PokeLab.ContentBrowser.Services;
using PokeLab.Services;
using PokeEngine.Core;

namespace PokeLab;

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