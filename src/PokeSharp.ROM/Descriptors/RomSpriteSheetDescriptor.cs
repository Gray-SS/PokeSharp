namespace PokeSharp.ROM.Descriptors;

public readonly struct RomSpriteSheetDescriptor : IRomDescriptor
{
    public int Offset { get; }
    public int FrameWidth { get; }
    public int FrameHeight { get; }
    public int FramesCount { get; }
    public int FrameDataLength { get; }
    public int DataLength { get; }
    public RomPaletteDescriptor Palette { get; }

    public RomAssetType Type => RomAssetType.SpriteSheet;

    public RomSpriteSheetDescriptor(int offset, RomPaletteDescriptor palette, int frameWidth, int frameHeight, int framesCount, int frameDataLength, int dataLength)
    {
        Offset = offset;
        Palette = palette;
        FrameWidth = frameWidth;
        FrameHeight = frameHeight;
        DataLength = dataLength;
        FramesCount = framesCount;
        FrameDataLength = frameDataLength;
    }
}