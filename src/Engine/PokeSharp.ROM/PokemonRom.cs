using PokeSharp.ROM.Platforms.Gba;

namespace PokeSharp.ROM;

public sealed class PokemonRom
{
    public RomInfo Info { get; }
    public GbaConfig Config { get; }

    public PokemonRom(RomInfo info, GbaConfig config)
    {
        Info = info;
        Config = config;
    }
}