namespace PokeTools.Assets.Objects.Raw;

public sealed record RawTexture(
    int Width,
    int Height,
    byte[] Data
);