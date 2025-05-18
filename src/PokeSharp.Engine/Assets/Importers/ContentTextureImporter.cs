using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PokeSharp.Engine.Assets.Importers;

public sealed class ContentTextureImporter : ContentAssetImporter<Texture2D>
{
    public ContentTextureImporter(ContentManager content, GraphicsDevice graphicsDevice) : base(content, graphicsDevice)
    {
    }

    public override Texture2D Import(AssetPipeline pipeline, AssetReference assetRef)
    {
        string path = assetRef.PayloadAs<string>();
        return  Content.Load<Texture2D>(path);
    }
}
