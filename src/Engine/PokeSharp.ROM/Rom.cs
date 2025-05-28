using PokeSharp.ROM.Platforms.Gba;
using PokeSharp.ROM.Services;

namespace PokeSharp.ROM;

public sealed class Rom
{
    public RomInfo Info { get; }
    public GbaConfig Config { get; }
    public IRomProvider Provider { get; }

    public Rom(RomInfo info, GbaConfig config, IRomProvider provider)
    {
        Info = info;
        Config = config;
        Provider = provider;
    }
}