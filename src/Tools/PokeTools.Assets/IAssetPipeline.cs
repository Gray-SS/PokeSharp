using PokeCore.IO;
using PokeCore.Common;

namespace PokeTools.Assets;

public interface IAssetPipeline
{
    IEnumerable<IAssetImporter> FindImportersForExtension(string extension);

    Task<Result> ImportAsync(IAssetImporter importer, VirtualPath path);
}