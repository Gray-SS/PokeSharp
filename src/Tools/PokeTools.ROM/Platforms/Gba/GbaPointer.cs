using System.Runtime.InteropServices;

namespace PokeTools.ROM.Platforms.Gba;

/// <summary>
/// Represents a Game Boy Advance memory pointer.
/// Handles the translation between GBA's virtual address space (0x08XXXXXX) and physical ROM addresses.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 4, Pack = 1)]
public readonly struct GbaPointer
{
    /// <summary>
    /// The virtual memory base address for GBA ROM pointers (0x08000000).
    /// All valid GBA pointers have this prefix.
    /// </summary>
    public const int VirtualMemoryBase = 0x08000000;

    /// <summary>
    /// The size of the GBA ROM pointer structure.
    /// </summary>
    public static readonly int Size = Marshal.SizeOf<GbaPointer>();

    /// <summary>
    /// Represents a null GBA pointer with a raw address of 0x00000000.
    /// </summary>
    /// <remarks>
    /// This special pointer can be used as a sentinel value to indicate:
    /// <list type="bullet">
    ///     <item><description>The end of a pointer array or linked structure</description></item>
    ///     <item><description>An uninitialized or invalid pointer reference</description></item>
    ///     <item><description>A placeholder for optional pointer parameters</description></item>
    /// </list>
    /// <br/>
    /// Note that this is not a valid GBA ROM pointer since it doesn't start with 0x08,
    /// but it follows the convention of using address 0 as a null pointer reference.
    /// </remarks>
    public static readonly GbaPointer Null = new(0x00000000);

    /// <summary>
    /// The raw address value as stored in the GBA ROM or RAM.
    /// </summary>
    public readonly int RawAddress;

    /// <summary>
    /// Gets the physical ROM address that this pointer references.
    /// <exception cref="InvalidOperationException">Thrown if this is not a valid GBA pointer.</exception>
    /// </summary>
    public int PhysicalAddress => GetPhysicalAddress();

    /// <summary>
    /// Indicates whether this address is a valid GBA pointer (starts with 0x08).
    /// </summary>
    public bool IsValid => IsGbaPointer(RawAddress);

    // /// <summary>
    // /// Gets the size in bytes of the pointed type.
    // /// </summary>
    // public static int ElementSize => Marshal.SizeOf<T>();

    /// <summary>
    /// Creates a new GBA pointer from a raw address without validation.
    /// </summary>
    /// <remarks>
    /// This constructor is private because it's only used internally for performing pointer arithmetic operations
    /// where validation has already occurred or isn't necessary. Making this private helps maintain the integrity
    /// of the API by ensuring all publicly created pointers are valid.
    /// <br/><br/>
    /// External code should use the public <see cref="FromRaw"/> method instead, which performs
    /// proper validation to ensure address integrity.
    /// </remarks>
    /// <param name="address">The raw address value.</param>
    private GbaPointer(int address)
    {
        RawAddress = address;
    }

    /// <summary>
    /// Creates a new validated GBA pointer from a raw address.
    /// </summary>
    /// <remarks>
    /// This factory method ensures that only valid GBA pointers (starting with 0x08) can be created.
    /// It performs validation checks before instantiating the pointer, throwing an exception if the address is invalid.
    /// <br/><br/>
    /// This is the recommended way to create GBA pointers from raw addresses in client code.
    /// </remarks>
    /// <param name="address">The raw address value that should be a valid GBA pointer (starting with 0x08).</param>
    /// <returns>A new validated GBA pointer of type T.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the address is not a valid GBA pointer.</exception>
    public static GbaPointer FromRaw(int address)
    {
        ValidatePointer(address);
        return new GbaPointer(address);
    }

    /// <summary>
    /// Creates a new GBA pointer from a physical ROM address by adding the virtual memory base.
    /// </summary>
    /// <param name="physicalAddress">The physical ROM address.</param>
    /// <returns>A GBA pointer to the specified physical address.</returns>
    public static GbaPointer FromPhysicalAddress(int physicalAddress)
    {
        return new GbaPointer(physicalAddress + VirtualMemoryBase);
    }

    /// <summary>
    /// Determines if this pointer references a valid location within the ROM data.
    /// </summary>
    /// <param name="romSize">The total size of the ROM data in bytes.</param>
    /// <returns>True if the pointer references a valid location within the ROM; otherwise, false.</returns>
    public bool ReferencesValidLocation(int romSize)
    {
        if (!TryGetPhysicalAddress(out int physicalAddress))
            return false;

        return physicalAddress >= 0x0 && physicalAddress < romSize;
    }

    /// <summary>
    /// Gets the physical ROM address that this pointer references.
    /// </summary>
    /// <returns>The physical ROM address.</returns>
    /// <exception cref="InvalidOperationException">Thrown if this is not a valid GBA pointer.</exception>
    public int GetPhysicalAddress()
    {
        if (!IsValid)
            throw new InvalidOperationException($"Cannot resolve physical address: Invalid GBA pointer format: 0x{RawAddress:X8}");

        return RawAddress - VirtualMemoryBase;
    }

    /// <summary>
    /// Attempts to get the physical ROM address that this pointer references.
    /// </summary>
    /// <param name="physicalAddress">When this method returns, contains the physical ROM address if successful; otherwise, contains the raw address value.</param>
    /// <returns>True if the physical address was successfully resolved; otherwise, false.</returns>
    public bool TryGetPhysicalAddress(out int physicalAddress)
    {
        if (!IsValid)
        {
            physicalAddress = RawAddress;
            return false;
        }

        physicalAddress = RawAddress - VirtualMemoryBase;
        return true;
    }

    /// <summary>
    /// Determines whether the specified address is a valid GBA pointer.
    /// </summary>
    /// <param name="address">The address to check.</param>
    /// <returns>True if the address is a valid GBA pointer; otherwise, false.</returns>
    public static bool IsGbaPointer(int address)
    {
        return (address & VirtualMemoryBase) == VirtualMemoryBase;
    }

    /// <summary>
    /// Validates that the specified address is a valid GBA pointer.
    /// </summary>
    /// <param name="address">The address to validate.</param>
    /// <exception cref="InvalidOperationException">Thrown if the address is not a valid GBA pointer.</exception>
    private static void ValidatePointer(int address)
    {
        if (!IsGbaPointer(address))
            throw new InvalidOperationException($"Invalid GBA pointer format: 0x{address:X8}. Valid GBA pointers must start with 0x08.");
    }

    /// <summary>
    /// Returns a string representation of the GBA pointer.
    /// </summary>
    /// <returns>A hexadecimal string representation of the pointer.</returns>
    public override string ToString()
    {
        return $"0x{RawAddress:X8} -> 0x{(IsValid ? PhysicalAddress.ToString("X8") : "Invalid")}";
    }

    #region Equality Members

    /// <summary>
    /// Determines whether this pointer is equal to another GBA pointer.
    /// </summary>
    /// <param name="other">The pointer to compare with this pointer.</param>
    /// <returns>True if the pointers are equal; otherwise, false.</returns>
    public bool Equals(GbaPointer other)
    {
        return RawAddress == other.RawAddress;
    }

    /// <summary>
    /// Determines whether this pointer is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with this pointer.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is GbaPointer pointer && Equals(pointer);
    }

    /// <summary>
    /// Returns a hash code for this pointer.
    /// </summary>
    /// <returns>A hash code for this pointer.</returns>
    public override int GetHashCode()
    {
        return RawAddress.GetHashCode();
    }

    /// <summary>
    /// Determines whether two GBA pointers are equal.
    /// </summary>
    /// <param name="left">The first pointer to compare.</param>
    /// <param name="right">The second pointer to compare.</param>
    /// <returns>True if the pointers are equal; otherwise, false.</returns>
    public static bool operator ==(GbaPointer left, GbaPointer right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two GBA pointers are not equal.
    /// </summary>
    /// <param name="left">The first pointer to compare.</param>
    /// <param name="right">The second pointer to compare.</param>
    /// <returns>True if the pointers are not equal; otherwise, false.</returns>
    public static bool operator !=(GbaPointer left, GbaPointer right)
    {
        return !(left == right);
    }

    #endregion

    #region Pointer Arithmetic

    /// <summary>
    /// Adds an offset in bytes to this GBA pointer.
    /// </summary>
    /// <param name="offset">The number of bytes to offset by.</param>
    /// <remarks>
    /// The returned pointer can be an invalid pointer so please check it's validity before using any operations
    /// </remarks>
    /// <returns>A new GBA pointer with the specified offset added.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the pointer is not valid.</exception>
    public GbaPointer Add(int offset)
    {
        if (!IsValid)
            throw new InvalidOperationException($"Cannot perform pointer arithmetic on invalid GBA pointer: 0x{RawAddress:X8}");

        return new GbaPointer(RawAddress + offset);
    }

    /// <summary>
    /// Increment the raw address by one byte to this GBA pointer.
    /// </summary>
    /// <remarks>
    /// The returned pointer can be an invalid pointer so please check it's validity before using any operations
    /// </remarks>
    /// <returns>A new GBA pointer with the incremented offset.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the pointer is not valid.</exception>
    public GbaPointer Inc()
    {
        if (!IsValid)
            throw new InvalidOperationException($"Cannot perform pointer arithmetic on invalid GBA pointer: 0x{RawAddress:X8}");

        return new GbaPointer(RawAddress + 1);
    }

    /// <summary>
    /// Adds an offset to this GBA pointer.
    /// </summary>
    /// <param name="count">The number of element to count by.</param>
    /// <remarks>
    /// The returned pointer can be an invalid pointer so please check it's validity before using any operations
    /// </remarks>
    /// <returns>A new GBA pointer with the specified count added.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the pointer is not valid.</exception>
    public GbaPointer Add<T>(int count) where T : struct
    {
        if (!IsValid)
            throw new InvalidOperationException($"Cannot perform pointer arithmetic on invalid GBA pointer: 0x{RawAddress:X8}");

        return new GbaPointer(RawAddress + count * Marshal.SizeOf<T>());
    }

    /// <summary>
    /// Increment the raw address by one byte to this GBA pointer.
    /// </summary>
    /// <remarks>
    /// The returned pointer can be an invalid pointer so please check it's validity before using any operations
    /// </remarks>
    /// <returns>A new GBA pointer with the incremented offset.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the pointer is not valid.</exception>
    public GbaPointer Inc<T>() where T : struct
    {
        if (!IsValid)
            throw new InvalidOperationException($"Cannot perform pointer arithmetic on invalid GBA pointer: 0x{RawAddress:X8}");

        return new GbaPointer(RawAddress + Marshal.SizeOf<T>());
    }

    /// <summary>
    /// Subtracts an offset in bytes from this GBA pointer.
    /// </summary>
    /// <param name="offset">The number of bytes to offset by.</param>
    /// <remarks>
    /// The returned pointer can be an invalid pointer so please check it's validity before using any operations
    /// </remarks>
    /// <returns>A new GBA pointer with the specified offset subtracted.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the pointer is not valid.</exception>
    public GbaPointer Subtract(int offset)
    {
        if (!IsValid)
            throw new InvalidOperationException($"Cannot perform pointer arithmetic on invalid GBA pointer: 0x{RawAddress:X8}");

        return new GbaPointer(RawAddress - offset);
    }

    /// <summary>
    /// Subtracts an offset in element count from this GBA pointer.
    /// </summary>
    /// <param name="count">The number of element to offset by.</param>
    /// <remarks>
    /// The returned pointer can be an invalid pointer so please check it's validity before using any operations
    /// </remarks>
    /// <returns>A new GBA pointer with the specified offset subtracted.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the pointer is not valid.</exception>
    public GbaPointer Subtract<T>(int count) where T : struct
    {
        if (!IsValid)
            throw new InvalidOperationException($"Cannot perform pointer arithmetic on invalid GBA pointer: 0x{RawAddress:X8}");

        return new GbaPointer(RawAddress - count * Marshal.SizeOf<T>());
    }


    /// <summary>
    /// Calculates the offset between two GBA pointers.
    /// </summary>
    /// <param name="other">The other GBA pointer.</param>
    /// <returns>The offset between the two pointers.</returns>
    /// <exception cref="InvalidOperationException">Thrown if either pointer is invalid.</exception>
    public int Offset(GbaPointer other)
    {
        if (!IsValid || !other.IsValid)
            throw new InvalidOperationException("Cannot calculate offset between invalid GBA pointers");

        return RawAddress - other.RawAddress;
    }

    /// <summary>
    /// Adds an offset in bytes to a GBA pointer.
    /// </summary>
    /// <param name="pointer">The GBA pointer.</param>
    /// <param name="offset">The offset to add.</param>
    /// <returns>A new GBA pointer with the specified offset added.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the pointer is not valid.</exception>
    public static GbaPointer operator +(GbaPointer pointer, int offset)
    {
        return pointer.Add(offset);
    }

    /// <summary>
    /// Subtracts an offset in bytes from a GBA pointer.
    /// </summary>
    /// <param name="pointer">The GBA pointer.</param>
    /// <param name="offset">The offset in bytes to subtract.</param>
    /// <returns>A new GBA pointer with the specified offset subtracted.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the pointer is not valid.</exception>
    public static GbaPointer operator -(GbaPointer pointer, int offset)
    {
        return pointer.Subtract(offset);
    }

    /// <summary>
    /// Calculates the bytes count between two GBA pointers.
    /// </summary>
    /// <param name="left">The first GBA pointer.</param>
    /// <param name="right">The second GBA pointer.</param>
    /// <returns>The number of bytes between the two pointers.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the pointer is not valid.</exception>
    public static int operator -(GbaPointer left, GbaPointer right)
    {
        return left.Offset(right);
    }

    /// <summary>
    /// Increments the pointer by one byte.
    /// </summary>
    /// <param name="pointer">The pointer to increment.</param>
    /// <returns>A new pointer that points to the next element.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the pointer is not valid.</exception>
    public static GbaPointer operator ++(GbaPointer pointer)
    {
        return pointer.Add(1);
    }

    /// <summary>
    /// Decrements the pointer by one byte.
    /// </summary>
    /// <param name="pointer">The pointer to decrement.</param>
    /// <returns>A new pointer that points to the previous element.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the pointer is not valid.</exception>
    public static GbaPointer operator --(GbaPointer pointer)
    {
        return pointer.Subtract(1);
    }

    #endregion
}