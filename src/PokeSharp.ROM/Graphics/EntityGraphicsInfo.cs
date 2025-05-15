namespace PokeSharp.ROM.Graphics;

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
    public IRomPalette Palette { get; }

    /// <summary>
    /// The sprite sheet of the entity
    /// </summary>
    public RomSpriteSheet SpriteSheet  { get; }

    /// <summary>
    /// The animations of this entity
    /// </summary>
    public GraphicsAnimation[] Animations { get; }

    public EntityGraphicsInfo(int width, int height, int shadowSize, IRomPalette palette, RomSpriteSheet spriteSheet, GraphicsAnimation[] animations)
    {
        Width = width;
        Height = height;
        ShadowSize = shadowSize;
        Palette = palette;
        SpriteSheet = spriteSheet;
        Animations = animations;
    }
}