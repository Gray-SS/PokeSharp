using Ninject;
using PokeSharp.Assets.Services;
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
        kernel.Bind<IAssetMetadataSerializer>().To<AssetMetadataSerializer>().InTransientScope();
        kernel.Bind<IAssetMetadataStore>().To<LibraryMetadataStore>().InSingletonScope();
    }
}