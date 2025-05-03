using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.DesktopGL.Core.Graphics;

namespace Pokemon.DesktopGL.Core.Extensions;

public static class SpriteExtensions
{
    public static Sprite LoadSprite(this ContentManager content, string assetName)
    {
        return Sprite.FromTexture(content.Load<Texture2D>(assetName));
    }
}