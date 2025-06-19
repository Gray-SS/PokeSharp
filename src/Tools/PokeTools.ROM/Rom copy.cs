using PokeEngine.ROM.Platforms.Gba;

namespace PokeEngine.ROM;

public sealed class Rom
{
    public RomInfo Info { get; }
    public GbaConfig Config { get; }
    public byte[] RawData => Info.RawData;

    public Rom(RomInfo info, GbaConfig config)
    {
        Info = info;
        Config = config;
    }
}