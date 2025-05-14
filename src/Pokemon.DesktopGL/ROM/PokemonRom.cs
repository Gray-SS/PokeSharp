namespace Pokemon.DesktopGL.ROM;

public sealed class PokemonRom
{
    public RomInfo Info { get; }
    public byte[] RomData { get; }
    public IPokemonRomProvider Provider { get; }

    public PokemonRom(RomInfo info, byte[] data, IPokemonRomProvider provider)
    {
        Info = info;
        RomData = data;

        Provider = provider;
    }

    public RomAssetsPack ExtractAssetPack()
        => Provider.ExtractAssetPack();
}