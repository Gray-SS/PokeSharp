namespace PokeSharp.ROM.LowLevel;

/// <summary>
/// This structure is not yet in use. It is intended to be used to represent a segmented (non-continuous) array of data.
/// This would make it possible to avoid allocating a continuous array of non-continuous data.
/// <br/> <br/>
/// <b>Use-cases</b>: Use this structure when you're working with <b>very large segmented arrays</b>.
/// In other cases, favour simplicity over memory optimisation to make the code more maintainable for everyone.
/// </summary>
public readonly struct SegmentedMemory
{
    private readonly MemorySegmentUnsafe[] _segments;

    public SegmentedMemory(MemorySegmentUnsafe[] segments)
    {
        _segments = segments;
    }
}