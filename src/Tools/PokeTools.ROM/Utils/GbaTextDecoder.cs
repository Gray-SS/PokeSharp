using System.Text;

namespace PokeEngine.ROM.Utils;

public static class GbaTextDecoder
{
    public static string Decode(ReadOnlySpan<byte> data)
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