namespace PokeSharp.ROM.Platforms.GBA;

public static class GbaRomOffsets
{
    public static Dictionary<GbaRomOffsetPointers, int> GetOffsetsByRomInfo(RomInfo romInfo)
    {
        if (!_offsets.TryGetValue(romInfo.GameCode, out Dictionary<GbaRomOffsetPointers, int>? offsets))
            throw new NotSupportedException($"The GBA ROM ({romInfo}) is currently not supported");

        EnsureFullImplementation(offsets);
        return offsets;
    }

    private static void EnsureFullImplementation(Dictionary<GbaRomOffsetPointers, int> offsets)
    {
        CheckMissingOffsets(offsets);
        CheckIfVirtualAddresses(offsets);
    }

    private static void CheckMissingOffsets(Dictionary<GbaRomOffsetPointers, int> offsets)
    {
        var missingOffsets = Enum.GetValues<GbaRomOffsetPointers>()
            .Where(offsetKind => !offsets.ContainsKey(offsetKind));

        if (missingOffsets.Any())
        {
            throw new NotSupportedException(
                $"The GBA ROM isn't fully supported. Missing offsets: {string.Join(", ", missingOffsets)}");
        }
    }

    private static void CheckIfVirtualAddresses(Dictionary<GbaRomOffsetPointers, int> offsets)
    {
        var physicalAddressesOffsets = offsets
            .Where(p => !GbaPointer.IsGbaPointer(p.Value))
            .Select(p => $"{p.Key}: 0x{p.Value:X8}");

        if (physicalAddressesOffsets.Any())
        {
            throw new NotSupportedException(
                $"Invalid offsets. Non pointer offsets found: {string.Join(", ", physicalAddressesOffsets)}");
        }
    }

    private static readonly Dictionary<string, Dictionary<GbaRomOffsetPointers, int>> _offsets = new()
    {
        // Pokemon Red (USA/English)
        ["BPRE"] = new Dictionary<GbaRomOffsetPointers, int>()
        {
            [GbaRomOffsetPointers.POKEMON_NAMES] = 0x08245EE0, // 11 bytes per name
            [GbaRomOffsetPointers.ABILITY_NAMES] = 0x08245EE0, // 13 bytes per name
            [GbaRomOffsetPointers.ATTACK_NAMES] = 0x08245EE0, // 13 bytes per name
            [GbaRomOffsetPointers.TYPE_NAMES] = 0x0824F1A0, // 7 bytes per name
            [GbaRomOffsetPointers.ICON_PALETTE_DATA] = 0x083D3E80, // 1 byte per icon, references one of three icon palettes
            [GbaRomOffsetPointers.ITEM_DATA] = 0x083DB028, // 44 bytes per item

            [GbaRomOffsetPointers.FRONT_POKEMON_SPRITES] = 0x082350AC, // Table of pointers
            [GbaRomOffsetPointers.BACK_POKEMON_SPRITES] = 0x0823654C, // Table of pointers
            [GbaRomOffsetPointers.ICON_SPRITES] = 0x083D37A0, // Table of pointers (not compressed)
            [GbaRomOffsetPointers.ITEM_SPRITES] = 0x083D4294, // 2 pointers (first one for the item sprite, second for item palette)

            [GbaRomOffsetPointers.BASE_STATS] = 0x08254784, // 28 bytes per Pokémon
            [GbaRomOffsetPointers.EVOLUTION_DATA] = 0x08259754, // Table of pointers
            [GbaRomOffsetPointers.FOOTPRINT_DATA] = 0x0843FAB0, // Table of pointers
            [GbaRomOffsetPointers.MOVESET_DATA] = 0x0825D7B4, // Table of pointers

            // Pokémon palettes are compressed using LZ77 algorithm
            [GbaRomOffsetPointers.REGULAR_PALETTES] = 0x0823730C, // Table of pointers
            [GbaRomOffsetPointers.SHINY_PALETTES] = 0x082380CC, // Table of pointers
            [GbaRomOffsetPointers.ICON_PALETTES] = 0x083D3740, // Table of pointers

            // 0839fdb0
            // gObjectEventGraphicsInfoPointers
            [GbaRomOffsetPointers.ENTITY_GRAPHICS_INFO] = 0x0839FDB0,
            [GbaRomOffsetPointers.ENTITY_PALETTES] = 0x083A5158,
        },
    };
}