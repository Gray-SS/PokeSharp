using Ninject.Modules;

namespace PokeSharp.Editor.UI;

public sealed class ImGuiModule : NinjectModule
{
    public override void Load()
    {
        Bind<ImGuiRenderer>().ToSelf().InSingletonScope();
    }
}