using PokeCore.Assets;
using PokeCore.Common;
using PokeTools.Assets.External.Annotations;

namespace PokeTools.Assets.External;

public interface IAssetProcessor
{
    AssetProcessorAttribute Metadata { get; }

    Result<IAsset> Process(Guid assetId, object rawAsset);
}