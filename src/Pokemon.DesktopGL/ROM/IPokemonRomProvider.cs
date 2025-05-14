using Pokemon.DesktopGL.ROM.Graphics;

namespace Pokemon.DesktopGL.ROM;

public interface IPokemonRomProvider
{
    byte[] RomData { get; }

    string ExtractPokemonName(int index);

    RomAssetsPack ExtractAssetPack();
    IGraphicsData ExtractItemIconSprite(int index);
    IGraphicsData ExtractPokemonIconSprite(int index);
    IGraphicsData ExtractFrontPokemonSprite(int index);
    IGraphicsData ExtractBackPokemonSprite(int index);
    EntityGraphicsInfo ExtractEntityGraphicsInfo(int index);
}