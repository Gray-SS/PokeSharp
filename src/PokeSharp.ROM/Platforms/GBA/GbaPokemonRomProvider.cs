using System.Text;
using System.Diagnostics;

namespace PokeSharp.ROM.Platforms.GBA;

public abstract class GbaPokemonRomProvider : PokemonRomProvider<GbaPointer>
{
    public Dictionary<GbaRomOffsetPointers, int> Offsets { get; }

    public GbaPokemonRomProvider(byte[] romData, Dictionary<GbaRomOffsetPointers, int> offsets) : base(romData)
    {
        Offsets = offsets ?? throw new ArgumentNullException(nameof(offsets));
    }

    public GbaPointer GetPointer(GbaRomOffsetPointers offsetType)
    {
        /*
            NOTE: If the assert fails, it means the ROM implementation is missing some offsets in the GbaRomOffsets class.

            In theory, this should never happen because an offset validation step is meant to catch such issues beforehand.
            If you encounter this failure, it's likely that the validation logic in GbaRomOffsets is incomplete or not functioning correctly.
        */
        Debug.Assert(Offsets.TryGetValue(offsetType, out int rawAddress), $"ROM implementation isn't fully implemented. Missing offset '{rawAddress}'.");
        return GbaPointer.Create(rawAddress);
    }

    protected static string DecodeGbaText(ReadOnlySpan<byte> data)
    {
        var sb = new StringBuilder();
        foreach (byte b in data)
        {
            if (b == 0xFF || b == 0x00) break;

            if (b >= 0xBB && b <= 0xD4)
                sb.Append((char)('a' + (b - 0xBB)));
            else if (b >= 0xA1 && b <= 0xBA)
                sb.Append((char)('A' + (b - 0xA1)));
            else
                sb.Append('?');
        }

        return sb.ToString();
    }
}