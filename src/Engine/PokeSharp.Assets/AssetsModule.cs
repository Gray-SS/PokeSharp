using Ninject;
using PokeSharp.Assets.VFS;
using PokeSharp.Core.Modules;

namespace PokeSharp.Assets;

public sealed class AssetsModule : Module
{
    public override string Name => "Assets";

    public override void Configure(IKernel kernel)
    {
        kernel.Bind<AssetPipeline>().ToSelf().InSingletonScope();
        kernel.Bind<IVirtualFileSystem>().To<VirtualFileSystem>().InSingletonScope();
    }

    public override void Load()
    {
    }
}