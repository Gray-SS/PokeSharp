using PokeSharp.ROM.Platforms.GBA;
using PokeSharp.ROM.Platforms.GBA.Providers;

namespace PokeSharp.ROM;

public static class PokemonRomProviderFactory
{
    public static IPokemonRomProvider GetProvider(RomInfo romInfo, byte[] romData)
    {
        return romInfo.Platform switch
        {
            RomPlatform.GBA => GetGbaRomProvider(romInfo, romData),

            _ => throw new NotSupportedException($"{romInfo.Platform} ROM are currently not supported."),
        };
    }

    private static IPokemonRomProvider GetGbaRomProvider(RomInfo romInfo, byte[] romData)
    {
        // NOTE: Implement other Pokémon GBA ROM here
        var offsets = GbaRomOffsets.GetOffsetsByRomInfo(romInfo);
        return romInfo.GameCode switch
        {
            "BPRE" => new PokemonFireRedProvider(romData, offsets),

            _ => throw new NotSupportedException("The GBA ROM isn't currently supported. No ROM provider was found.")
        };
    }
}