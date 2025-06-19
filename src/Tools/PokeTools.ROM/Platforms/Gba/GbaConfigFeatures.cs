namespace PokeEngine.ROM.Platforms.Gba;

[Flags]
public enum GbaConfigFeatures
{
    None = 0,
    Minimal = 1 << 0,
    SpeciesLoading = 1 << 1,
    EntityGraphics = 1 << 2,
}