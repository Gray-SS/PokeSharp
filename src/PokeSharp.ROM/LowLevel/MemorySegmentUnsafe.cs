using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PokeSharp.ROM.LowLevel;

/// <summary>
/// In principle, this structure is only used with in the context of the <see cref="SegmentedMemory"/> structure. <br/>
/// Never use this structure outside <see cref="SegmentedMemory"/> as this would mean working with a Span that is safer and favoured.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct MemorySegmentUnsafe
{
    public readonly byte* ptr;
    public readonly int size;

    public MemorySegmentUnsafe(ReadOnlySpan<byte> span)
    {
        this.size = span.Length;

        ref byte first = ref MemoryMarshal.GetReference(span);
        ptr = (byte*)Unsafe.AsPointer(ref first);
    }
}