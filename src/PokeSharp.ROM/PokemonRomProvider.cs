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

    public abstract string Load(NameDescriptor desc);
    public abstract IRomTexture Load(SpriteDescriptor desc);
    public abstract IRomPalette Load(PaletteDescriptor desc);
}