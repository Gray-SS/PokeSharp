using Microsoft.Xna.Framework.Graphics;
using PokeSharp.Engine.Graphics;

namespace PokeSharp.Engine.Assets.Processors;

public sealed class SpriteProcessor : AssetProcessor<Texture2D, Sprite>
{
    public override Sprite Process(Texture2D input)
    {
        return new Sprite(input);
    }
}