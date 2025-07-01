using PokeCore.Assets;
using PokeCore.Common;

namespace PokeTools.Assets;

public interface IAssetPipeline
{
    AssetType AssetType { get; }
    IAssetImporter Importer { get; }
    IAssetProcessor Processor { get; }
    IAssetSerializer Serializer { get; }

    Task<Result> BuildAsync(AssetMetadata metadata, Stream inputStream, Stream outputStream);
}