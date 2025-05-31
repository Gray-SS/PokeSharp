using Microsoft.Xna.Framework.Graphics;
using PokeSharp.Assets;
using PokeSharp.Assets.VFS;
using PokeSharp.Rendering.Assets.Raw;

namespace PokeSharp.Rendering.Assets;

public sealed class SpriteImporter : AssetImporter<RawSprite>
{
    public override Type ProcessorType => typeof(SpriteProcessor);

    private readonly GraphicsDevice _graphicsDevice;

    public SpriteImporter(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public override bool CanImport(VirtualPath path)
    {
        return path.Extension is ".png" or ".jpeg" or ".jpg";
    }

    public override RawSprite Import(IVirtualFile file)
    {
        using var stream = file.OpenRead();
        Texture2D texture = Texture2D.FromStream(_graphicsDevice, stream);

        return new RawSprite(texture, null);
    }
}