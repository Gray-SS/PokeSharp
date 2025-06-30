using PokeCore.IO;
using PokeCore.Common;
using PokeCore.Assets;

namespace PokeTools.Assets;

public interface IAssetPipeline
{
    Task<Result> NewAsync(AssetType assetType, VirtualPath outputPath);
    Task<Result> BuildAsync(VirtualPath inputPath, VirtualPath outputPath);
    Result BuildBundle(VirtualPath dirPath, string bundleName);
}