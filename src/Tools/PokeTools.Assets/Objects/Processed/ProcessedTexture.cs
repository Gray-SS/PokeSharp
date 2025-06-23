namespace PokeTools.Assets.Objects.Processed;

public sealed record ProcessedTexture(
    int Width,
    int Height,
    byte[] Data
);