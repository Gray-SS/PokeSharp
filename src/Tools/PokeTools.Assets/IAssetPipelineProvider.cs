using PokeCore.Assets;

namespace PokeTools.Assets;

public interface IAssetPipelineProvider
{
    IAssetImporter? GetImporter(string extension);
    IAssetImporter? GetImporter(AssetType assetType);
    IAssetProcessor? GetProcessor(AssetType assetType);
    IAssetCompiler? GetCompiler(AssetType assetType);

    IAssetPipeline? GetPipeline(string extension);
    IAssetPipeline? GetPipeline(AssetType assetType);
}