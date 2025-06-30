namespace PokeTools.Assets.Intermediate;

public sealed record RawTexture(
    int Width,
    int Height,
    byte[] Data
);