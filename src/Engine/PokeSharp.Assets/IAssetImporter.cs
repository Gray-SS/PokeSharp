namespace PokeSharp.Assets;

public interface IAssetImporter
{
    bool CanImport(string ext);
    object? Import(string path);
}