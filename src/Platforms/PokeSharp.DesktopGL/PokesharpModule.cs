using Ninject.Modules;
using PokeSharp.DesktopGL.GUI;

namespace PokeSharp.DesktopGL;

public sealed class PokesharpModule : NinjectModule
{
    public override void Load()
    {
        Bind<AssetsViewerHook>().ToSelf().InSingletonScope();
    }
}