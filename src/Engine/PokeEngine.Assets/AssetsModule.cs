using Ninject;
using PokeEngine.Assets.Services;
using PokeEngine.Assets.VFS;
using PokeEngine.Assets.VFS.Services;
using PokeCore.Hosting.Modules;

namespace PokeEngine.Assets;

public sealed class AssetsModule : Module
{
    public override string Name => "Assets";

    public override void Configure(IKernel kernel)
    {
        kernel.Bind<AssetPipeline>().ToSelf().InSingletonScope();
        kernel.Bind<IVirtualFileSystem>().To<VirtualFileSystem>().InSingletonScope();
        kernel.Bind<IVirtualVolumeManager>().To<VirtualVolumeManager>().InSingletonScope();
        kernel.Bind<IAssetMetadataSerializer>().To<AssetMetadataSerializer>().InTransientScope();
    }
}