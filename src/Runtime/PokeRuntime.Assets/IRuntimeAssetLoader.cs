using PokeCore.Assets;

namespace PokeRuntime.Assets;

public interface IRuntimeAssetLoader
{
    AssetType AssetType { get; }

    IRuntimeAsset Load(Guid assetId, BinaryReader reader);
}