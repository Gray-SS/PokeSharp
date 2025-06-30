namespace PokeTools.Assets.External.Intermediate;

public sealed record RawTexture(
    int Width,
    int Height,
    byte[] Data
);