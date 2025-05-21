using PokeSharp.ROM.Descriptors;
using PokeSharp.ROM.Graphics;

namespace PokeSharp.ROM;

public abstract class PokemonRomProvider : IPokemonRomProvider
{
    public byte[] RomData { get; }

    public PokemonRomProvider(byte[] romData)
    {
        RomData = romData;
    }

    public abstract RomAssetsPack ExtractAssetPack();

    public abstract string Load(RomNameDescriptor desc);
    public abstract IRomTexture Load(RomSpriteDescriptor desc);
    public abstract IRomPalette Load(RomPaletteDescriptor desc);
    public abstract RomAnimation Load(RomAnimationDescriptor desc);
    public abstract RomSpriteSheet Load(RomSpriteSheetDescriptor desc);
    public abstract EntityGraphicsInfo Load(RomEntityGraphicsDescriptor desc);
}