namespace PokeSharp.Assets;

public interface IAssetImporter
{
    Type ProcessorType { get; }

    bool CanImport(string ext);
    object? Import(string path);
}