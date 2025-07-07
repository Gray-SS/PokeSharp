using PokeCore.Assets;
using PokeCore.Common.Results;
using PokeTools.Assets.Annotations;

namespace PokeTools.Assets.Pipeline.Abstractions;

public interface IAssetProcessor
{
    AssetProcessorAttribute Metadata { get; }

    Result<IAsset> Process(Guid assetId, object rawAsset);
}