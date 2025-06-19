using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PokeTools.Assets.Raw;

public sealed record RawSprite(
    Texture2D Texture,
    Rectangle? SourceRect
);