using PokeCore.Common;
using YamlDotNet.Serialization;

namespace PokeTools.Assets;

public abstract class AuthoredAssetImporter<TRaw> : AssetImporter<TRaw>
    where TRaw : class, IRawAsset
{
    public override Result<TRaw> Import(Stream stream)
    {
        using var reader = new StreamReader(stream);
        string yaml = reader.ReadToEnd();

        if (string.IsNullOrWhiteSpace(yaml))
            return Result<TRaw>.Failure(new Error("File content was null or empty."));


        return Result.Catch(() =>
        {
            return new Deserializer().Deserialize<TRaw>(yaml);
        });
    }
}