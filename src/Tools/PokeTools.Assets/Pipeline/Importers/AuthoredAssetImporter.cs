using YamlDotNet.Core;
using PokeCore.Common.Results;
using PokeCore.Common.Serializations;
using PokeTools.Assets.Core;

namespace PokeTools.Assets.Pipeline.Importers;

public abstract class AuthoredAssetImporter<TRaw>(
    IYamlSerializer yamlSerializer
) : AssetImporter<TRaw>
    where TRaw : class, IRawAsset
{
    public override Result<TRaw> Import(Stream stream)
    {
        using var reader = new StreamReader(stream);
        string yaml = reader.ReadToEnd();

        if (string.IsNullOrWhiteSpace(yaml))
            return Result.Failure("Asset file content is empty");

        try
        {
            TRaw rawAsset = yamlSerializer.Deserialize<TRaw>(yaml);
            return rawAsset;
        }
        catch (YamlException ex)
        {
            return Result.Failure($"Invalid asset format: {ex.Message}");
        }
    }
}