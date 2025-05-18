using PokeSharp.ROM.Descriptors;

namespace PokeSharp.ROM;

public sealed class RomAssetsPack
{
    // Sounds

    // Palettes
    public List<RomPaletteDescriptor> Palettes { get; }

    // Sprites
    public List<RomSpriteDescriptor> PokemonFrontSprites { get; }

    public List<RomSpriteDescriptor> PokemonBackSprites { get; }

    // Names
    public List<RomNameDescriptor> PokemonNames { get; }

    // Spritesheets

    // Animations

    // Objects
    public List<RomEntityGraphicsDescriptor> EntitiesGraphicsInfo { get; }

    // Pokemons Data

    // Items Data

    public RomAssetsPack()
    {
        Palettes = new List<RomPaletteDescriptor>();

        PokemonBackSprites = new List<RomSpriteDescriptor>();
        PokemonFrontSprites = new List<RomSpriteDescriptor>();
        PokemonNames = new List<RomNameDescriptor>();
        EntitiesGraphicsInfo = new List<RomEntityGraphicsDescriptor>();
        // NpcsEntityGraphicsInfo = new List<EntityGraphicsInfo>();
    }

    public RomNameDescriptor GetPokemonName(int pokedexId)
    {
        return PokemonNames[pokedexId];
    }

    public RomSpriteDescriptor GetPokemonFrontSprite(int pokedexId)
    {
        return PokemonFrontSprites[pokedexId];
    }

    public RomSpriteDescriptor GetPokemonBackSprite(int pokedexId)
    {
        return PokemonBackSprites[pokedexId];
    }
}