using PokeSharp.ROM.Platforms.Gba;

namespace PokeSharp.ROM;

public sealed class Rom
{
    public RomInfo Info { get; }
    public GbaConfig Config { get; }

    public Rom(RomInfo info, GbaConfig config)
    {
        Info = info;
        Config = config;
    }
}