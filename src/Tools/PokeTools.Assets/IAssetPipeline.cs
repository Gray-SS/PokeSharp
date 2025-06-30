using PokeCore.IO;
using PokeCore.Common;

namespace PokeTools.Assets;

public interface IAssetPipeline
{
    Task<Result> BuildAsync(VirtualPath inputPath, VirtualPath outputPath);

    // Result BuildBundle(VirtualPath dirPath, string bundleName);
}