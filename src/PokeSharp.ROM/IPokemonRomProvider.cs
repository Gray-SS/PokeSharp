using PokeSharp.ROM.Descriptors;
using PokeSharp.ROM.Graphics;

namespace PokeSharp.ROM;

public interface IPokemonRomProvider
{
    byte[] RomData { get; }

    string Load(NameDescriptor desc);
    IRomTexture Load(SpriteDescriptor desc);
    IRomPalette Load(PaletteDescriptor desc);
    RomAnimation Load(AnimationDescriptor desc);
    RomSpriteSheet Load(SpriteSheetDescriptor desc);
    EntityGraphicsInfo Load(EntityGraphicsDescriptor desc);

    RomAssetsPack ExtractAssetPack();
}