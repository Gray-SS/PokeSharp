using PokeCore.IO;

namespace PokeEngine.Assets;

public abstract class AssetImporter<T> : IAssetImporter
{
    public abstract Type ProcessorType { get; }
    public abstract string SupportedExtensions { get; }

    public abstract T Import(IVirtualFile file);

    object? IAssetImporter.Import(IVirtualFile file)
    {
        return Import(file);
    }
}