using PokeCore.IO;
using PokeCore.Assets;
using PokeTools.Assets.Pipeline.Abstractions;
using PokeTools.Assets.Services.Abstractions;
using PokeCore.Common.Results;

namespace PokeTools.Assets.Services;

public sealed class AssetTypeResolver(
    IEnumerable<IAssetImporter> importers
) : IAssetTypeResolver
{
    private readonly IEnumerable<IAssetImporter> _importers = importers;

    public AssetType ResolveTypeFromPath(VirtualPath path)
    {
        string extension = path.Extension;
        foreach (IAssetImporter importer in _importers)
        {
            if (importer.Metadata.SupportedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                return importer.Metadata.AssetType;
        }

        Result<int> re = 10;

        return AssetType.None;
    }
}