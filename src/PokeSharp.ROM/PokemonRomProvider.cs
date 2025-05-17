using PokeSharp.ROM.Descriptors;
using PokeSharp.ROM.Graphics;

namespace PokeSharp.ROM;

public abstract class PokemonRomProvider<TPointer> : IPokemonRomProvider
    where TPointer : unmanaged
{
    public byte[] RomData { get; }
    public RomReader<TPointer> Reader { get; }

    public PokemonRomProvider(byte[] romData)
    {
        RomData = romData;
        Reader = new RomReader<TPointer>(romData);
    }

    public abstract RomAssetsPack ExtractAssetPack();
    public abstract string ExtractPokemonName(int index);
    public abstract IRomTexture ExtractItemIconSprite(int index);
    public abstract IRomTexture ExtractPokemonIconSprite(int index);
    public abstract IRomTexture ExtractFrontPokemonSprite(int index);
    public abstract IRomTexture ExtractBackPokemonSprite(int index);
    public abstract EntityGraphicsInfo ExtractEntityGraphicsInfo(int index);

    public abstract string Load(NameDescriptor desc);
    public abstract IRomTexture Load(SpriteDescriptor desc);
    public abstract IRomPalette Load(PaletteDescriptor desc);
}