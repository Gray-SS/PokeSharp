using PokeSharp.ROM.Graphics;

namespace PokeSharp.ROM;

public interface IPokemonRomProvider
{
    byte[] RomData { get; }

    string ExtractPokemonName(int index);

    RomAssetsPack ExtractAssetPack();
    IRomTexture ExtractItemIconSprite(int index);
    IRomTexture ExtractPokemonIconSprite(int index);
    IRomTexture ExtractFrontPokemonSprite(int index);
    IRomTexture ExtractBackPokemonSprite(int index);
    EntityGraphicsInfo ExtractEntityGraphicsInfo(int index);
}