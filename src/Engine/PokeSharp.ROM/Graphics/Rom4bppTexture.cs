using System.Drawing;
using System.Runtime.InteropServices;

namespace PokeSharp.ROM.Graphics;

public sealed class Rom4bppTexture : IRomTexture
{
    public int Width { get; }

    public int Height { get; }

    public byte[] PixelsData { get; }

    public IRomPalette Palette { get; }

    public RomPixelFormat PixelFormat => RomPixelFormat.Bpp4;

    public Rom4bppTexture(int width, int height, byte[] pixelsData, IRomPalette palette)
    {
        Width = width;
        Height = height;
        PixelsData = pixelsData;
        Palette = palette;
    }

    public Color[] ToRGBA()
    {
        int tileWidth = 8;
        int tileHeight = 8;
        int columns = Width / tileWidth;
        int rows = Height / tileHeight;

        Color[] pixels = new Color[Width * Height];

        int tileIndex = 0;
        for (int ty = 0; ty < rows; ty++)
        {
            for (int tx = 0; tx < columns; tx++)
            {
                for (int row = 0; row < tileHeight; row++)
                {
                    for (int col = 0; col < tileWidth; col += 2)
                    {
                        byte packed = PixelsData[tileIndex++];
                        int x = tx * tileWidth + col;
                        int y = ty * tileHeight + row;

                        byte lowNibble = (byte)(packed & 0xF);
                        byte highNibble = (byte)(packed >> 4);

                        pixels[y * Width + x] = Palette.Data[lowNibble];
                        pixels[y * Width + x + 1] = Palette.Data[highNibble];
                    }
                }
            }
        }

        return pixels;
    }
}
