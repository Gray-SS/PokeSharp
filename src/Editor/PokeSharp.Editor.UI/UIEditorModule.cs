using Ninject.Modules;
using PokeSharp.Editor.UI.Services;

namespace PokeSharp.Editor.UI;

public sealed class UIEditorModule : NinjectModule
{
    public override void Load()
    {
        Bind<ImGuiRenderer>().ToSelf().InSingletonScope();
        Bind<IGuiHookDispatcher>().To<GuiHookDispatcher>().InSingletonScope();
    }
}