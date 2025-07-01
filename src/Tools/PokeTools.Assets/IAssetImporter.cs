using PokeCore.Common;
using PokeTools.Assets.Annotations;

namespace PokeTools.Assets;

public interface IAssetImporter
{
    AssetImporterAttribute Metadata { get; }
    ImportParameter[] Parameters { get; }

    Result<IRawAsset> Import(Stream stream);
}