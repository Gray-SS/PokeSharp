using PokeCore.Assets;
using PokeTools.Assets.Authored.Annotations;

namespace PokeTools.Assets.Authored;

public interface IAssetLoader
{
    Type DescriptorType { get; }
    AssetLoaderAttribute Metadata { get; }

    IAsset Load(Guid assetId, object descriptor);
}