using System.Diagnostics;
using System.Collections.Generic;
using System;

namespace Pokemon.DesktopGL.ROM.GBA;

public abstract class GbaPokemonRomProvider : PokemonRomProvider
{
    public const int FILE_OFFSET = 0x08000000;

    public Dictionary<GbaRomOffsetKind, int> Offsets { get; }

    public GbaPokemonRomProvider(RomInfo romInfo, byte[] romData, Dictionary<GbaRomOffsetKind, int> offsets) : base(romInfo, romData)
    {
        Offsets = offsets;
    }

    public int GetOffset(GbaRomOffsetKind offset)
    {
        Debug.Assert(Offsets.TryGetValue(offset, out int offsetValue), $"No offset for '{offset}' is defined.");
        return offsetValue;
    }

    public int GetFileOffset(GbaRomOffsetKind offset)
        => GetFileOffset(GetOffset(offset));

    public int ReadPointer(GbaRomOffsetKind offset)
    {
        int fileOffset = GetFileOffset(offset);
        int pointer = BitConverter.ToInt32(Data, fileOffset);

        return pointer;
    }

    public static int GetFileOffset(int offset)
        => offset - FILE_OFFSET;
}