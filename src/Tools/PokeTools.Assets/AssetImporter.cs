using PokeCore.Common;
using PokeCore.IO;

namespace PokeTools.Assets;

public abstract class AssetImporter<TRaw> : IAssetImporter
    where TRaw : class
{
    public abstract Type ProcessorType { get; }
    public abstract string[] SupportedExtensions { get; }

    public abstract Result<TRaw> Import(Stream stream);

    Result<object> IAssetImporter.Import(Stream stream)
    {
        return Import(stream);
    }
}