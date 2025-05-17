using PokeSharp.ROM.Descriptors;
using PokeSharp.ROM.Graphics;

namespace PokeSharp.ROM;

public interface IPokemonRomProvider
{
    byte[] RomData { get; }

    string ExtractPokemonName(int index);

    string Load(NameDescriptor desc);
    IRomTexture Load(SpriteDescriptor desc);
    IRomPalette Load(PaletteDescriptor desc);

    RomAssetsPack ExtractAssetPack();
    IRomTexture ExtractItemIconSprite(int index);
    IRomTexture ExtractPokemonIconSprite(int index);
    IRomTexture ExtractFrontPokemonSprite(int index);
    IRomTexture ExtractBackPokemonSprite(int index);
    EntityGraphicsInfo ExtractEntityGraphicsInfo(int index);
}