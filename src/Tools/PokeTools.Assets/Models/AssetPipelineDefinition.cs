using PokeTools.Assets.Pipeline.Abstractions;

namespace PokeTools.Assets.Models;

public sealed record AssetPipelineDefinition(
    IAssetImporter Importer,
    IAssetProcessor Processor,
    IAssetSerializer Serializer
);