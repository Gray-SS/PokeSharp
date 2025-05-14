using Pokemon.DesktopGL.ROM.Graphics;

namespace Pokemon.DesktopGL.ROM;

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
    public abstract IGraphicsData ExtractItemIconSprite(int index);
    public abstract IGraphicsData ExtractPokemonIconSprite(int index);
    public abstract IGraphicsData ExtractFrontPokemonSprite(int index);
    public abstract IGraphicsData ExtractBackPokemonSprite(int index);
    public abstract EntityGraphicsInfo ExtractEntityGraphicsInfo(int index);
}