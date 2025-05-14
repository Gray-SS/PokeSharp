using System;
using System.Buffers.Binary;
using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.ROM.Graphics;

namespace Pokemon.DesktopGL.ROM.GBA;

public sealed class GbaGraphicsPalette : IGraphicsPalette
{
    public const int COLOR_COUNT = 16;
    public const int BYTES_COUNT = COLOR_COUNT * 2;

    public int ColorCount => COLOR_COUNT;

    public Color[] Data { get; }

    public GbaGraphicsPalette(Color[] data)
    {
        Data = data;
    }

    public static GbaGraphicsPalette FromRawData(ReadOnlySpan<byte> paletteData, bool transparent)
    {
        if (paletteData.Length != COLOR_COUNT * 2)
            throw new InvalidOperationException("The palette must be a 32 bytes length array.");

        Color[] colors = new Color[COLOR_COUNT];
        for (int i = 0; i < COLOR_COUNT; i++)
        {
            if (transparent && i == 0)
            {
                colors[0] = Color.Transparent;
                continue;
            }

            ReadOnlySpan<byte> entrySpan = paletteData.Slice(i * 2, sizeof(short));
            ushort entry = BinaryPrimitives.ReadUInt16LittleEndian(entrySpan);
            int r = (entry & 0x1F) << 3;
            int g = ((entry >> 5) & 0x1F) << 3;
            int b = ((entry >> 10) & 0x1F) << 3;
            colors[i] = new Color(r, g, b);
        }

        return new GbaGraphicsPalette(colors);
    }
}