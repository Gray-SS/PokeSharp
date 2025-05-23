using Ninject;
using Ninject.Syntax;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Modules;
using PokeSharp.Editor.Miscellaneous;
using PokeSharp.Editor.UI;
using PokeSharp.Editor.Views;
using YamlDotNet.Serialization.NodeTypeResolvers;

namespace PokeSharp.Editor;

public sealed class EditorModule : Module
{
    public override string Name => "Editor";

    public override void Configure(IKernel kernel)
    {
        kernel.Bind<EditorConsoleBuffer>().ToSelf().InSingletonScope();
    }

    public override void ConfigureLogging(LoggerSettings settings, IResolutionRoot container)
    {
        EditorConsoleBuffer buffer = container.Get<EditorConsoleBuffer>();
        settings.AddOutput(buffer);
    }

    public override void Register(IModuleLoader loader)
    {
        loader.RegisterModule(new EditorUiModule());
    }
}
