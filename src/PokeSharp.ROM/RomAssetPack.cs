using PokeSharp.ROM.Descriptors;

namespace PokeSharp.ROM;

/// <summary>
/// This asset pack must have all the abstracted assets required
/// to start loading them and use them to build & run the project
/// </summary>
public sealed class RomAssetsPack
{
    // Sounds

    // Sprites
    public List<SpriteDescriptor> PokemonFrontSprites { get; }

    public List<SpriteDescriptor> PokemonBackSprites { get; }

    // Palettes
    public List<PaletteDescriptor> Palettes { get; }

    // Names
    public List<NameDescriptor> PokemonNames { get; }

    // Spritesheets

    // Animations

    // Objects
    // public EntityGraphicsInfo PlayerEntityGraphicsInfo { get; set; }
    // public List<EntityGraphicsInfo> NpcsEntityGraphicsInfo { get; set; }

    // Pokemons Data

    // Items Data

    public RomAssetsPack()
    {
        Palettes = new List<PaletteDescriptor>();

        PokemonBackSprites = new List<SpriteDescriptor>();
        PokemonFrontSprites = new List<SpriteDescriptor>();
        PokemonNames = new List<NameDescriptor>();
        // NpcsEntityGraphicsInfo = new List<EntityGraphicsInfo>();
    }
}