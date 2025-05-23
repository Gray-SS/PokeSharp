using Ninject;
using PokeSharp.Core.Modules;
using PokeSharp.Editor.UI.Services;

namespace PokeSharp.Editor.UI;

public sealed class EditorUiModule : Module
{
    public override string ModuleName => "Editor.UI";

    public override void Configure(IKernel kernel)
    {
        kernel.Bind<ImGuiRenderer>().ToSelf().InSingletonScope();
        kernel.Bind<IGuiHookDispatcher>().To<GuiHookDispatcher>().InSingletonScope();
    }
}