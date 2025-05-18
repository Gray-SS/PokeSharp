namespace PokeSharp.ROM.Descriptors;

public readonly struct AnimationDescriptor : IRomDescriptor
{
    public int Offset { get; }

    public RomAssetType Type => RomAssetType.Animation;

    public AnimationDescriptor(int offset)
    {
        Offset = offset;
    }
}
