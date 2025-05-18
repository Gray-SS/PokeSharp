namespace PokeSharp.ROM.Descriptors;

public readonly struct RomAnimationDescriptor : IRomDescriptor
{
    public int Offset { get; }

    public RomAssetType Type => RomAssetType.Animation;

    public RomAnimationDescriptor(int offset)
    {
        Offset = offset;
    }
}
