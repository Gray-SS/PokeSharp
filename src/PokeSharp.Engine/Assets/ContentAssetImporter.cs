using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PokeSharp.Engine.Assets;

public abstract class ContentAssetImporter<T> : AssetImporter<T> where T : class
{
    public ContentManager Content { get; }

    public ContentAssetImporter(ContentManager content, GraphicsDevice graphicsDevice) : base(graphicsDevice)
    {
        Content = content;
    }

    public override bool CanImport(AssetReference assetRef)
    {
        if (assetRef.Source != AssetSource.Content)
            return false;

        if (assetRef.Payload is not string)
            throw new InvalidOperationException("Expected a string path payload for content asset.");

        return true;
    }
}