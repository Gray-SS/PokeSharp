using PokeCore.Assets;
using PokeCore.Common.Results;
using PokeCore.Common.Results.Extensions;

namespace PokeRuntime.Assets;

public abstract class RuntimeAssetLoader<TAsset> : IRuntimeAssetLoader
    where TAsset : class
{
    public abstract AssetType AssetType { get; }

    public abstract Result<TAsset> Load(Guid assetId, BinaryReader reader);

    Result<object> IRuntimeAssetLoader.Load(Guid assetId, BinaryReader reader)
    {
        return Load(assetId, reader).Cast<TAsset, object>();
    }
}