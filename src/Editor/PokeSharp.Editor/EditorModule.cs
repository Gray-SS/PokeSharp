using Ninject;
using PokeSharp.Core.Modules;
using PokeSharp.Editor.UI;

namespace PokeSharp.Editor;

public sealed class EditorModule : Module
{
    public override string ModuleName => "Editor";

    public override void Configure(IKernel kernel)
    {
    }

    public override void RegisterSubmodules(IModuleLoader loader)
    {
        loader.RegisterModule(new EditorUiModule());
    }
}
