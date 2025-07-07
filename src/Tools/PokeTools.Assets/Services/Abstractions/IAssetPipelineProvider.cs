using PokeCore.Assets;
using PokeCore.Common.Results;
using PokeTools.Assets.Models;

namespace PokeTools.Assets.Services.Abstractions;

public interface IAssetPipelineProvider
{
    Result<AssetPipelineDefinition> GetPipeline(AssetType assetType);
}