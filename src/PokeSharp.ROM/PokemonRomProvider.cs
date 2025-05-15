using PokeSharp.ROM.Graphics;

namespace PokeSharp.ROM;

public abstract class PokemonRomProvider : IPokemonRomProvider
{
    public byte[] RomData { get; }
    public RomReader Reader { get; }
    public IRomAddressResolver AddressResolver { get; }

    public PokemonRomProvider(byte[] romData, IRomAddressResolver addressResolver)
    {
        RomData = romData;
        Reader = new RomReader(romData);
        AddressResolver = addressResolver;
    }

    public abstract RomAssetsPack ExtractAssetPack();
    public abstract string ExtractPokemonName(int index);
    public abstract IRomTexture ExtractItemIconSprite(int index);
    public abstract IRomTexture ExtractPokemonIconSprite(int index);
    public abstract IRomTexture ExtractFrontPokemonSprite(int index);
    public abstract IRomTexture ExtractBackPokemonSprite(int index);
    public abstract EntityGraphicsInfo ExtractEntityGraphicsInfo(int index);
}