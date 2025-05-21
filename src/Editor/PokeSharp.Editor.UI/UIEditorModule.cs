using Ninject.Modules;
using PokeSharp.Core.Services;
using PokeSharp.Editor.UI.Services;

namespace PokeSharp.Editor.UI;

public sealed class EditorUiModule : NinjectModule
{
    public override void Load()
    {
        Bind<ImGuiRenderer>().ToSelf().InSingletonScope();
        Bind<IGuiHookDispatcher>().To<GuiHookDispatcher>().InSingletonScope();
    }
}