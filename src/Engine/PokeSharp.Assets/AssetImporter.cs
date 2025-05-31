using PokeSharp.Assets.VFS;

namespace PokeSharp.Assets;

public abstract class AssetImporter<T> : IAssetImporter
{
    public abstract Type ProcessorType { get; }

    public abstract bool CanImport(VirtualPath path);
    public abstract T Import(IVirtualFile file);

    object? IAssetImporter.Import(IVirtualFile file)
    {
        return Import(file);
    }
}