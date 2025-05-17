using PokeSharp.ROM.Descriptors;

namespace PokeSharp.ROM;

/// <summary>
/// This asset pack must have all the abstracted assets required
/// to start loading them and use them to build & run the project
/// </summary>
public sealed class RomAssetsPack
{
    // Sounds

    // Palettes
    public List<PaletteDescriptor> Palettes { get; }

    // Sprites
    public List<SpriteDescriptor> PokemonFrontSprites { get; }

    public List<SpriteDescriptor> PokemonBackSprites { get; }
    // Names
    public List<NameDescriptor> PokemonNames { get; }

    // Spritesheets

    // Animations

    // Objects
    public List<EntityGraphicsDescriptor> EntitiesGraphicsInfo { get; }

    // Pokemons Data

    // Items Data

    public RomAssetsPack()
    {
        Palettes = new List<PaletteDescriptor>();

        PokemonBackSprites = new List<SpriteDescriptor>();
        PokemonFrontSprites = new List<SpriteDescriptor>();
        PokemonNames = new List<NameDescriptor>();
        EntitiesGraphicsInfo = new List<EntityGraphicsDescriptor>();
        // NpcsEntityGraphicsInfo = new List<EntityGraphicsInfo>();
    }
}