using PokeCore.Common.Results;
using PokeTools.Assets.Annotations;
using PokeTools.Assets.Core;
using PokeTools.Assets.Models;

namespace PokeTools.Assets.Pipeline.Abstractions;

public interface IAssetImporter
{
    AssetImporterAttribute Metadata { get; }
    ImportParameter[] Parameters { get; }

    Result<IRawAsset> Import(Stream stream);
}