using PokeCore.IO;
using PokeCore.Common;

namespace PokeTools.Assets;

public interface IAssetBuildServices
{
    Task<Result> BuildAsync(VirtualPath inputPath, VirtualPath outputPath);

    Result BuildManifest(VirtualPath dirPath);
}