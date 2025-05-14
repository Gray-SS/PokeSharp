namespace Pokemon.DesktopGL.ROM;

public interface IRomAddressResolver
{
    bool IsVirtual(int address);
    bool IsPhysical(int address);

    int ToPhysical(int virtualAddress);
    bool TryToPhysical(int virtualAddress, out int physicalAddress);
}