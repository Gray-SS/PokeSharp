using PokeCore.Assets;

namespace PokeRuntime.Assets;

public abstract class RuntimeAssetLoader<TAsset> : IRuntimeAssetLoader
    where TAsset : IRuntimeAsset
{
    public abstract AssetType AssetType { get; }

    public abstract TAsset Load(Guid assetId, BinaryReader reader);

    IRuntimeAsset IRuntimeAssetLoader.Load(Guid assetId, BinaryReader reader)
    {
        return Load(assetId, reader);
    }
}