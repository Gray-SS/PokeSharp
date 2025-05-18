using Microsoft.Xna.Framework.Graphics;

namespace PokeSharp.Engine.Assets;

public abstract class AssetImporter<T> : IAssetImporter where T : class
{
    public GraphicsDevice GraphicsDevice { get; }

    public AssetImporter(GraphicsDevice graphicsDevice)
    {
        GraphicsDevice = graphicsDevice;
    }

    public abstract bool CanImport(AssetReference assetRef);
    public abstract T Import(AssetPipeline pipeline, AssetReference assetRef);

    object IAssetImporter.Import(AssetPipeline pipeline, AssetReference assetRef)
    {
        return Import(pipeline, assetRef);
    }
}