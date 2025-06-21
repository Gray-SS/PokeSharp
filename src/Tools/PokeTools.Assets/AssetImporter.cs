using PokeCore.Common;
using PokeCore.IO;

namespace PokeTools.Assets;

public abstract class AssetImporter<T> : IAssetImporter
    where T : class
{
    public abstract Type ProcessorType { get; }
    public abstract string SupportedExtensions { get; }

    public abstract Result<T> Import(IVirtualFile file);

    Result<object> IAssetImporter.Import(IVirtualFile file)
    {
        return Import(file);
    }
}