namespace Pokemon.DesktopGL.ROM.Graphics;

public readonly struct EntityGraphicsInfo
{
    /// <summary>
    /// Width in pixels
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Height in pixels
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// The shadow size
    /// </summary>
    public int ShadowSize { get; }

    /// <summary>
    /// The palette of the entity
    /// </summary>
    public IGraphicsPalette Palette { get; }

    /// <summary>
    /// The palette of the entity
    /// </summary>
    public IGraphicsData SpriteSheet { get; }

    public EntityGraphicsInfo(int width, int height, int shadowSize, IGraphicsPalette palette, IGraphicsData spriteSheet)
    {
        Width = width;
        Height = height;
        ShadowSize = shadowSize;
        Palette = palette;
        SpriteSheet = spriteSheet;
    }
}