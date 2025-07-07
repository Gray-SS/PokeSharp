using System.Drawing;
using Microsoft.Xna.Framework.Graphics;

namespace PokeRuntime.Assets;

public sealed class Sprite
{
    public Texture2D? Texture { get; }
    public Rectangle? TextureRegion { get; }

    public Sprite(Texture2D? texture, Rectangle? textureRegion)
    {
        Texture = texture;
        TextureRegion = textureRegion;
    }
}