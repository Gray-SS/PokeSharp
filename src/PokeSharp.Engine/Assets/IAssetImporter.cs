namespace PokeSharp.Engine.Assets;

public interface IAssetImporter
{
    bool CanImport(AssetReference assetRef);
    object Import(AssetPipeline pipeline, AssetReference assetRef);
}