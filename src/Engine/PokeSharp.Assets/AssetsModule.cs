using Ninject;
using PokeSharp.Core.Modules;

namespace PokeSharp.Assets;

public sealed class AssetsModule : Module
{
    public override string Name => "Assets";

    public override void Configure(IKernel kernel)
    {
        kernel.Bind<AssetPipeline>().ToSelf().InSingletonScope();
    }
}