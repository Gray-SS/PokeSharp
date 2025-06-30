namespace PokeRuntime.Assets;

public interface IRuntimeAssetLoader<TAsset> where TAsset : IRuntimeAsset
{
    TAsset Load(Guid assetId, BinaryReader reader);
}