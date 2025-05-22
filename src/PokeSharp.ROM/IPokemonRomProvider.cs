using PokeSharp.ROM.Descriptors;
using PokeSharp.ROM.Graphics;

namespace PokeSharp.ROM;

public interface IPokemonRomProvider
{
    byte[] RomData { get; }

    string Load(RomNameDescriptor desc);
    IRomTexture Load(RomSpriteDescriptor desc);
    IRomPalette Load(RomPaletteDescriptor desc);
    RomAnimation Load(RomAnimationDescriptor desc);
    RomSpriteSheet Load(RomSpriteSheetDescriptor desc);
    EntityGraphicsInfo Load(RomEntityGraphicsDescriptor desc);

    RomAssetsPack ExtractAssetPack();
}