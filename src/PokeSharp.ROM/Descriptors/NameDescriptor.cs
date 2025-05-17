namespace PokeSharp.ROM.Descriptors;

public enum NameKind
{
    Pokemom
}

public readonly struct NameDescriptor : IRomDescriptor
{
    public int Address { get; }
    public int Length { get; }
    public NameKind Kind { get; }
    public RomAssetType Type => RomAssetType.Name;

    public NameDescriptor(int offset, int length, NameKind kind)
    {
        Kind = kind;
        Address = offset;
        Length = length;
    }
}