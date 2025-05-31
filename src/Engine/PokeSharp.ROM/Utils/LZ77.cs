namespace PokeSharp.ROM.Utils;

public static class LZ77
{
    public static byte[] DecompressLZ77(ReadOnlySpan<byte> data, int offset)
    {
        if (data[offset] != 0x10)
            throw new InvalidOperationException("Data is not LZ77 compressed.");

        int decompressedSize = data[offset + 1] | (data[offset + 2] << 8) | (data[offset + 3] << 16);
        var result = new List<byte>(decompressedSize);

        int pos = offset + 4;

        while (result.Count < decompressedSize)
        {
            byte flag = data[pos++];
            for (int i = 0; i < 8 && result.Count < decompressedSize; i++)
            {
                if ((flag & (0x80 >> i)) == 0)
                {
                    result.Add(data[pos++]);
                }
                else
                {
                    byte b1 = data[pos++];
                    byte b2 = data[pos++];
                    int length = ((b1 >> 4) & 0xF) + 3;
                    int disp = ((b1 & 0xF) << 8) | b2;

                    int copyPos = result.Count - disp - 1;
                    for (int j = 0; j < length; j++)
                    {
                        result.Add(result[copyPos + j]);
                    }
                }
            }
        }

        return [.. result];
    }
}