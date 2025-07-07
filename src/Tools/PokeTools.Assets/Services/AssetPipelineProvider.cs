using PokeCore.Assets;
using PokeCore.Common.Results;
using PokeCore.Common.Results.Extensions;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;
using PokeTools.Assets.Models;
using PokeTools.Assets.Pipeline.Abstractions;
using PokeTools.Assets.Services.Abstractions;

namespace PokeTools.Assets.Services;

public sealed class AssetPipelineProvider(
    IServiceResolver services
) : IAssetPipelineProvider
{
    public Result<AssetPipelineDefinition> GetPipeline(AssetType assetType)
    {
        return from importer in GetImporter(assetType)
               from processor in GetProcessor(assetType)
               from serializer in GetSerializer(assetType)
               select new AssetPipelineDefinition(importer, processor, serializer);
    }

    private Result<IAssetImporter> GetImporter(AssetType assetType)
    {
        return Result.FromNullable(services.GetServices<IAssetImporter>()
            .FirstOrDefault(x => x.Metadata.AssetType == assetType), $"Assets of type '{assetType}' aren't supported");
    }

    private Result<IAssetProcessor> GetProcessor(AssetType assetType)
    {
        return Result.FromNullable(services.GetServices<IAssetProcessor>()
            .FirstOrDefault(x => x.Metadata.AssetType == assetType), $"Assets of type '{assetType}' aren't supported");
    }

    private Result<IAssetSerializer> GetSerializer(AssetType assetType)
    {
        return Result.FromNullable(services.GetServices<IAssetSerializer>()
            .FirstOrDefault(x => x.Metadata.AssetType == assetType), $"Assets of type '{assetType}' aren't supported");
    }
}