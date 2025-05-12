using System;
using Pokemon.DesktopGL.ROM.GBA;

namespace Pokemon.DesktopGL.ROM;

public static class RomLoaderFactory
{
    public static IRomLoader CreateLoader(Span<byte> romData)
    {
        if (IsGbaRom(romData))
            return new GbaRomLoader();

        throw new NotSupportedException("Invalid or unsupported ROM type.");
    }

    private static bool IsGbaRom(ReadOnlySpan<byte> romData)
    {
        if (romData.Length <= 0xB2)
            return false;

        return romData[0xB2] == 0x96;
    }

    // NOTE: Add supports for other type of ROMs here
}