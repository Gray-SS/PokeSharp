using Ninject;
using Ninject.Modules;
using PokeSharp.Editor.UI;

namespace PokeSharp.Editor;

public sealed class EditorModule : NinjectModule
{
    public override void Load()
    {
        Kernel.Load<EditorUiModule>();
    }
}
