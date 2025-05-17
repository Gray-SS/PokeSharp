using PokeSharp.ROM.Descriptors;
using PokeSharp.ROM.Graphics;

namespace PokeSharp.ROM;

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

    public string Load(NameDescriptor descriptor)
        => Provider.Load(descriptor);

    public IRomTexture Load(SpriteDescriptor descriptor)
        => Provider.Load(descriptor);

    public IRomPalette Load(PaletteDescriptor descriptor)
        => Provider.Load(descriptor);
}