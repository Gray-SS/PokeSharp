using PokeCore.Assets;
using PokeCore.Common.Results;

namespace PokeRuntime.Assets;

public interface IRuntimeAssetLoader
{
    AssetType AssetType { get; }

    Result<object> Load(Guid assetId, BinaryReader reader);
}