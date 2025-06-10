using System.Drawing;

namespace PokeEngine.ROM.Graphics;

public interface IRomTexture : IIndexedResource
{
    int Width { get; }
    int Height { get; }
    byte[] PixelsData { get; }
    IRomPalette Palette { get; }

    Color[] ToRGBA();
}