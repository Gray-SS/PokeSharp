namespace PokeSharp.ROM.Descriptors;

public readonly struct SpriteSheetDescriptor : IRomDescriptor
{
    public int Offset { get; }
    public int FramesCount { get; }
    public int DataLength { get; }

    public RomAssetType Type => RomAssetType.SpriteSheet;

    public SpriteSheetDescriptor(int framesCount, int dataLength)
    {
        DataLength = dataLength;
        FramesCount = framesCount;
    }
}