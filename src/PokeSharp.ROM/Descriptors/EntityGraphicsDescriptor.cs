namespace PokeSharp.ROM.Descriptors;

public readonly struct EntityGraphicsDescriptor
{
    public int Offset { get; }

    public PaletteDescriptor Palette { get; }

    public SpriteSheetDescriptor SpriteSheet { get; }

    public AnimationDescriptor[] Animations { get; }

    public EntityGraphicsDescriptor(int offset, PaletteDescriptor palette, SpriteSheetDescriptor spriteSheet, AnimationDescriptor[] animations)
    {
        Offset = offset;
        Palette = palette;
        SpriteSheet = spriteSheet;
        Animations = animations;
    }
}