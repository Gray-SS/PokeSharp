namespace PokeSharp.ROM.Platforms.Gba;

public sealed class GbaConfig
{
    public int Version { get; set; } = -1;
    public string VersionText => Version == -1 ? string.Empty : $"v1.{Version}";
    public string Name { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string GameCode { get; set; } = string.Empty;
    public string VolumeScheme { get; set; } = string.Empty;
    public GbaPokemonsConfig Pokemons { get; set; } = null!;
    public GbaEntitiesConfig Entities { get; set; } = null!;

    public bool SupportsMinimal =>
        !string.IsNullOrEmpty(Name)
        && !string.IsNullOrEmpty(Language)
        && !string.IsNullOrEmpty(GameCode)
        && !string.IsNullOrEmpty(VolumeScheme)
        && Version >= 0;

    public bool SupportsSpeciesLoading =>
        Pokemons.Count > 0
        && Pokemons.FrontSprites != 0
        && Pokemons.BackSprites != 0;

    public bool SupportsEntityGraphics =>
        Entities.Graphics != 0;

    public GbaConfigFeatures GetSupportedFeatures()
    {
        var features = GbaConfigFeatures.None;

        if (SupportsMinimal) features |= GbaConfigFeatures.Minimal;
        if (SupportsSpeciesLoading) features |= GbaConfigFeatures.SpeciesLoading;
        if (SupportsEntityGraphics) features |= GbaConfigFeatures.EntityGraphics;

        return features;
    }

    public IEnumerable<GbaConfigFeatures> GetMissingFeatures()
    {
        if (!SupportsMinimal) yield return GbaConfigFeatures.Minimal;
        if (!SupportsSpeciesLoading) yield return GbaConfigFeatures.SpeciesLoading;
        if (!SupportsEntityGraphics) yield return GbaConfigFeatures.EntityGraphics;
    }

    public bool IsSupporting(GbaConfigFeatures feature)
    {
        return (GetSupportedFeatures() & feature) != 0;
    }

    public bool IsFullySupported()
    {
        return !GetMissingFeatures().Any();
    }

    public override string ToString()
    {
        string name = string.IsNullOrWhiteSpace(Name) ? "<Unnamed ROM>" : Name;
        return $"{name} {VersionText} {Language}";
    }
}

public sealed class GbaPokemonsConfig
{
    public int Count { get; set; }
    public int Names { get; set; }
    public int NameLength { get; set; }
    public int FrontSprites { get; set; }
    public int BackSprites { get; set; }
}

public sealed class GbaEntitiesConfig
{
    public int Graphics { get; set; }
}