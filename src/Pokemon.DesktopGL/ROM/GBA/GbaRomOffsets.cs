using System;
using System.Collections.Generic;
using System.Linq;

namespace Pokemon.DesktopGL.ROM.GBA;

public static class GbaRomOffsets
{
    private static readonly Dictionary<string, Dictionary<GbaRomOffsetKind, int>> _offsets = new()
    {
        ["BPRE"] = new Dictionary<GbaRomOffsetKind, int>() {
            [GbaRomOffsetKind.POKEMON_NAMES] = 0x08245EE0,
            [GbaRomOffsetKind.ABILITY_NAMES] = 0x08245EE0,
            [GbaRomOffsetKind.ATTACK_NAMES] = 0x08245EE0,
            [GbaRomOffsetKind.BACK_POKEMON_SPRITE] = 0x08245EE0,
            [GbaRomOffsetKind.FRONT_FRONT_SPRITE] = 0x08245EE0,
            [GbaRomOffsetKind.BASE_STATS] = 0x08245EE0,
            [GbaRomOffsetKind.EVOLUTION_DATA] = 0x08245EE0,
            [GbaRomOffsetKind.FOOTPRINT_DATA] = 0x08245EE0,
            [GbaRomOffsetKind.MOVESET_DATA] = 0x08245EE0,
            [GbaRomOffsetKind.REGULAR_PALETTES] = 0x08245EE0,
            [GbaRomOffsetKind.SHINY_PALETTES] = 0x08245EE0,
            [GbaRomOffsetKind.TYPE_NAMES] = 0x08245EE0,
        }
    };

    public static Dictionary<GbaRomOffsetKind, int> GetOffsetsByGameCode(string gameCode)
    {
        if (!_offsets.TryGetValue(gameCode, out Dictionary<GbaRomOffsetKind, int> offsets))
            throw new NotSupportedException("The GBA ROM isn't currently supported");

        EnsureFullImplementation(offsets);
        return offsets;
    }

    private static void EnsureFullImplementation(Dictionary<GbaRomOffsetKind, int> offsets)
    {
        var missingOffsetKinds = Enum.GetValues<GbaRomOffsetKind>()
            .Where(offsetKind => !offsets.ContainsKey(offsetKind));

        if (missingOffsetKinds.Any())
        {
            throw new NotSupportedException(
                $"The GBA ROM isn't fully supported. Missing offsets: {string.Join(", ", missingOffsetKinds)}");
        }
    }
}