using PokeCore.Common;
using PokeTools.Assets.Annotations;

namespace PokeTools.Assets;

public interface IAssetImporter
{
    AssetImporterAttribute Metadata { get; }
    ImportParameter[] Parameters { get; }

    Result<object> Import(Stream stream);
}