namespace Pokemon.DesktopGL.ROM;

public abstract class PokemonRomProvider : IPokemonRomProvider
{
    public byte[] Data { get; }
    public RomInfo RomInfo { get; }

    public PokemonRomProvider(RomInfo romInfo, byte[] romData)
    {
        RomInfo = romInfo;
        Data = romData;
    }

    public abstract string GetPokemonName(int index);
}