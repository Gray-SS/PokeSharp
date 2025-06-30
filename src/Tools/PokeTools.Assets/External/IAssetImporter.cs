using PokeCore.Common;
using PokeTools.Assets.External.Annotations;

namespace PokeTools.Assets.External;

public interface IAssetImporter
{
    AssetImporterAttribute Metadata { get; }
    ImportParameter[] Parameters { get; }

    Result<object> Import(Stream stream);
}