using Microsoft.Xna.Framework.Graphics;

namespace Pokemon.DesktopGL.ROM.Graphics;

public interface IGraphicsData
{
    int Width { get; }
    int Height { get; }

    GraphicsFormat Format { get; }

    byte[] PixelsData { get; }
    IGraphicsPalette Palette { get; }

    Texture2D GenerateTexture(GraphicsDevice graphicsDevice);
}