namespace PokeSharp.ROM.Descriptors;

public readonly struct RomEntityGraphicsDescriptor
{
    public int Offset { get; }

    public RomPaletteDescriptor Palette { get; }

    public RomSpriteSheetDescriptor SpriteSheet { get; }

    public RomAnimationDescriptor[] Animations { get; }

    public RomEntityGraphicsDescriptor(int offset, RomPaletteDescriptor palette, RomSpriteSheetDescriptor spriteSheet, RomAnimationDescriptor[] animations)
    {
        Offset = offset;
        Palette = palette;
        SpriteSheet = spriteSheet;
        Animations = animations;
    }
}