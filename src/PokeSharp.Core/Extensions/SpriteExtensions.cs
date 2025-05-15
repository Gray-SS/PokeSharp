using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PokeSharp.Core.Graphics;

namespace PokeSharp.Core.Extensions;

public static class SpriteExtensions
{
    public static Sprite LoadSprite(this ContentManager content, string assetName)
    {
        return Sprite.FromTexture(content.Load<Texture2D>(assetName));
    }
}