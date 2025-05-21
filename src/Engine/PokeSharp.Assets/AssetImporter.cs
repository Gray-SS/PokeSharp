namespace PokeSharp.Assets;

public abstract class AssetImporter<T> : IAssetImporter
{
    public abstract bool CanImport(string ext);

    public abstract T Import(string path);

    object? IAssetImporter.Import(string path)
    {
        return Import(path);
    }
}