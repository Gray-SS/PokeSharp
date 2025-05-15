using System.Buffers.Binary;
using System.Diagnostics;
using System.Drawing;

namespace PokeSharp.ROM.Graphics;

public sealed class Rom4bppPalette : IRomPalette
{
    public const int COLOR_COUNT = 16;
    public const int BYTES_COUNT = COLOR_COUNT * 2;

    public Color[] Data { get; }
    public RomPixelFormat PixelFormat => RomPixelFormat.Bpp4;

    public Rom4bppPalette(Color[] data)
    {
        Debug.Assert(data.Length == COLOR_COUNT, $"A 4bpp indexed palette is expecting {COLOR_COUNT} colors but received {data.Length}.");

        Data = data;
    }

    public static Rom4bppPalette FromRawData(ReadOnlySpan<byte> rawData, bool transparent)
    {
        if (rawData.Length != BYTES_COUNT)
            throw new InvalidOperationException($"A 4bpp indexed palette is expecting {BYTES_COUNT} bytes of data but received {rawData.Length} bytes.");

        Color[] colors = new Color[COLOR_COUNT];
        for (int i = 0; i < COLOR_COUNT; i++)
        {
            if (transparent && i == 0)
            {
                colors[0] = Color.FromArgb(0);
                continue;
            }

            ReadOnlySpan<byte> entrySpan = rawData.Slice(i * 2, sizeof(short));
            ushort entry = BinaryPrimitives.ReadUInt16LittleEndian(entrySpan);
            int r = (entry & 0x1F) << 3;
            int g = ((entry >> 5) & 0x1F) << 3;
            int b = ((entry >> 10) & 0x1F) << 3;
            colors[i] = Color.FromArgb(r, g, b);
        }

        return new Rom4bppPalette(colors);
    }
}