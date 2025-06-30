using PokeCore.Assets;
using PokeCore.Common;
using PokeTools.Assets.Annotations;

namespace PokeTools.Assets;

public interface IAssetProcessor
{
    AssetProcessorAttribute Metadata { get; }

    Result<IAsset> Process(Guid assetId, object rawAsset);
}