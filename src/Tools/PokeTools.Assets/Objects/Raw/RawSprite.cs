using Microsoft.Xna.Framework;

namespace PokeTools.Assets.Objects.Raw;

public sealed record RawSprite(
    int Width,
    int Height,
    byte[] RawData,
    Rectangle? SourceRect
);