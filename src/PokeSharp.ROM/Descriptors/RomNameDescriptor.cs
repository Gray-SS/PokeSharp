namespace PokeSharp.ROM.Descriptors;

public enum NameKind
{
    Pokemom
}

public readonly struct RomNameDescriptor : IRomDescriptor
{
    public int Address { get; }
    public int Length { get; }
    public NameKind Kind { get; }
    public RomAssetType Type => RomAssetType.Name;

    public RomNameDescriptor(int offset, int length, NameKind kind)
    {
        Kind = kind;
        Address = offset;
        Length = length;
    }
}