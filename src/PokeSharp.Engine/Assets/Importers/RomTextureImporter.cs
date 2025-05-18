using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PokeSharp.Engine.Managers;
using PokeSharp.ROM.Descriptors;
using PokeSharp.ROM.Graphics;

namespace PokeSharp.Engine.Assets.Importers;

public sealed class TextureImporter : RomAssetImporter<Texture2D>
{
    public TextureImporter(RomManager romManager, GraphicsDevice graphicsDevice) : base(romManager, graphicsDevice)
    {
    }

    public override bool CanImport(AssetReference assetRef)
    {
        if (!base.CanImport(assetRef))
            return false;

        if (assetRef.Payload is not RomSpriteDescriptor)
            throw new InvalidOperationException($"Expected a '{nameof(RomSpriteDescriptor)}' payload for content asset.");

        return true;
    }

    public override Texture2D Import(AssetPipeline pipeline, AssetReference assetRef)
    {
        RomSpriteDescriptor descriptor = assetRef.PayloadAs<RomSpriteDescriptor>();

        IRomTexture romTexture = Rom.Load(descriptor);
        Color[] textureData = romTexture.ToRGBA()
            .Select(x => new Color(x.R, x.G, x.B, x.A))
            .ToArray();

        Texture2D texture = new Texture2D(GraphicsDevice, romTexture.Width, romTexture.Height);
        texture.SetData(textureData);

        return texture;
    }
}