using Ninject.Modules;

namespace PokeSharp.Assets;

public sealed class AssetsModule : NinjectModule
{
    public override void Load()
    {
        Bind<AssetPipeline>().ToSelf().InSingletonScope();
    }
}