using PokeCore.IO;
using PokeCore.Common;

namespace PokeTools.Assets;

public sealed class AssetPipeline : IAssetPipeline
{
    public Task<Result> ImportAsync(VirtualPath path)
    {
        // Importer une resource depuis un path virtuel

        

        return Result.SuccessAsync();
    }
}