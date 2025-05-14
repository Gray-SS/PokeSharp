using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.DesktopGL.ROM.Graphics;

namespace Pokemon.DesktopGL.ROM.GBA;

public sealed class Gba4bppGraphicsData : IGraphicsData
{
    public int Width { get; }
    public int Height { get; }

    public GraphicsFormat Format => GraphicsFormat.Bpp4;

    public byte[] PixelsData { get; }
    public IGraphicsPalette Palette { get; }

    public Gba4bppGraphicsData(int width, int height, byte[] pixelsData, IGraphicsPalette palette)
    {
        Width = width;
        Height = height;
        Palette = palette;
        PixelsData = pixelsData;
    }

    public Texture2D GenerateTexture(GraphicsDevice graphics)
    {
        int tileWidth = 8;
        int tileHeight = 8;
        int tilesPerRow = Width / tileWidth;

        Color[] pixels = new Color[Width * Height];

        int tileIndex = 0;
        for (int ty = 0; ty < Height / tileHeight; ty++)
        {
            for (int tx = 0; tx < tilesPerRow; tx++)
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

        var texture = new Texture2D(graphics, Width, Height);
        texture.SetData(pixels);
        return texture;
    }
}
