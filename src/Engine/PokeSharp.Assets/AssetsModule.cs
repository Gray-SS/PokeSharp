using Ninject;
using PokeSharp.Assets.Services;
using PokeSharp.Assets.VFS;
using PokeSharp.Assets.VFS.Services;
using PokeSharp.Core.Modules;

namespace PokeSharp.Assets;

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