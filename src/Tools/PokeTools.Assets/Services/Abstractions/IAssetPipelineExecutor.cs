using PokeCore.Assets;
using PokeCore.Common.Results;
using PokeTools.Assets.Core;
using PokeTools.Assets.Models;

namespace PokeTools.Assets.Services.Abstractions;

public interface IAssetPipelineExecutor
{
    Task<Result<IAsset>> ExecuteAsync(AssetPipelineDefinition definition, AssetMetadata metadata, Stream inputStream, Stream outputStream);
}