using System;

namespace Pokemon.DesktopGL.ROM.GBA;

public sealed class GbaRomAddressResolver : IRomAddressResolver
{
    private const int VIRTUAL_BASE = 0x08000000;

    public static readonly GbaRomAddressResolver Default = new();

    public bool IsVirtual(int address)
        => (address & VIRTUAL_BASE) == VIRTUAL_BASE;

    public bool IsPhysical(int address)
        => !IsVirtual(address);

    public int ToPhysical(int virtualAddress)
    {
        if (!IsVirtual(virtualAddress))
            throw new InvalidOperationException($"Expected a virtual address, got physical: 0x{virtualAddress:X8}");

        return virtualAddress - VIRTUAL_BASE;
    }

    public bool TryToPhysical(int virtualAddress, out int physicalAddress)
    {
        if (!IsVirtual(virtualAddress))
        {
            physicalAddress = virtualAddress;
            return false;
        }

        physicalAddress = ToPhysical(virtualAddress);
        return true;
    }
}