using System.Reflection;
using PokeCore.Common;
using PokeTools.Assets.Annotations;

namespace PokeTools.Assets;

public abstract class AssetImporter<TRaw> : IAssetImporter
    where TRaw : class, IRawAsset
{
    public AssetImporterAttribute Metadata { get; }
    public ImportParameter[] Parameters { get; }

    public AssetImporter()
    {
        Metadata = LoadMetadata();
        Parameters = LoadParameters();
    }

    private AssetImporterAttribute LoadMetadata()
    {
        return GetType().GetCustomAttribute<AssetImporterAttribute>() ??
            throw new InvalidOperationException($"The asset importer '{GetType().Name}' is not annotated with '{nameof(AssetImporterAttribute)}'");
    }

    private ImportParameter[] LoadParameters()
    {
        var parameters = new List<ImportParameter>();

        foreach (PropertyInfo property in GetType().GetProperties())
        {
            ImportParameterAttribute? attr = property.GetCustomAttribute<ImportParameterAttribute>();
            if (attr == null) continue;

            parameters.Add(new ImportParameter(this, property, attr));
        }

        return parameters.ToArray();
    }

    public abstract Result<TRaw> Import(Stream stream);

    Result<IRawAsset> IAssetImporter.Import(Stream stream)
    {
        return Import(stream).Map<IRawAsset>(x => x);
    }
}