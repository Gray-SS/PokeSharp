using System;
using System.Buffers.Binary;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace PokeSharp.ROM;

public class RomReader
{
    public byte[] Data { get; }

    public RomReader(byte[] data)
    {
        Data = data;
    }

    public virtual sbyte ReadSByte(int baseOffset)
    {
        EnsurePositionInBounds(baseOffset, 1);
        return (sbyte)Data[baseOffset];
    }

    public virtual sbyte ReadSByte(int baseOffset, int offset)
        => ReadSByte(baseOffset + offset);

    public virtual byte ReadByte(int baseOffset)
    {
        EnsurePositionInBounds(baseOffset, 1);
        return Data[baseOffset];
    }

    public virtual byte ReadByte(int baseOffset, int offset)
        => ReadByte(baseOffset+ offset);

    public virtual ushort ReadUInt16(int baseOffset)
    {
        EnsurePositionInBounds(baseOffset, 2);
        return BinaryPrimitives.ReadUInt16LittleEndian(Data.AsSpan(baseOffset, 2));
    }

    public virtual ushort ReadUInt16(int baseOffset, int offset)
        => ReadUInt16(baseOffset + offset);

    public virtual short ReadInt16(int baseOffset)
    {
        EnsurePositionInBounds(baseOffset, 2);
        return BinaryPrimitives.ReadInt16LittleEndian(Data.AsSpan(baseOffset, 2));
    }

    public virtual short ReadInt16(int baseOffset, int offset)
        => ReadInt16(baseOffset + offset);

    public virtual uint ReadUInt32(int baseOffset)
    {
        EnsurePositionInBounds(baseOffset, 4);
        return BinaryPrimitives.ReadUInt32LittleEndian(Data.AsSpan(baseOffset, 4));
    }

    public virtual uint ReadUInt32(int baseOffset, int offset)
        => ReadUInt32(baseOffset + offset);

    public virtual int ReadInt32(int baseOffset)
    {
        EnsurePositionInBounds(baseOffset, 4);
        return BinaryPrimitives.ReadInt32LittleEndian(Data.AsSpan(baseOffset, 4));
    }

    public virtual int ReadInt32(int baseOffset, int offset)
        => ReadInt32(baseOffset + offset);

    public virtual ulong ReadUInt64(int baseOffset)
    {
        EnsurePositionInBounds(baseOffset, 8);
        return BinaryPrimitives.ReadUInt64LittleEndian(Data.AsSpan(baseOffset, 8));
    }

    public virtual ulong ReadUInt64(int baseOffset, int offset)
        => ReadUInt64(baseOffset + offset);

    public virtual long ReadInt64(int baseOffset)
    {
        EnsurePositionInBounds(baseOffset, 8);
        return BinaryPrimitives.ReadInt64LittleEndian(Data.AsSpan(baseOffset, 8));
    }

    public virtual long ReadInt64(int baseOffset, int offset)
        => ReadInt64(baseOffset + offset);

    public virtual ReadOnlySpan<byte> ReadString(int baseOffset, int length)
    {
        EnsurePositionInBounds(baseOffset, length);

        ReadOnlySpan<byte> bytes = MemoryMarshal.AsBytes(Data.AsSpan(baseOffset, length));

        // This is used to trim the string to the actual length of the string if null terminated
        int nullIndex = bytes.IndexOf((byte)0);
        if (nullIndex >= 0)
            bytes = bytes[..nullIndex];

        return bytes;
    }

    public virtual ReadOnlySpan<byte> ReadString(int baseOffset, int offset, int length)
        => ReadString(baseOffset + offset, length);

    public virtual ReadOnlySpan<byte> ReadSpan(int baseOffset, int length)
    {
        EnsurePositionInBounds(baseOffset, length);

        ReadOnlySpan<byte> bytes = MemoryMarshal.AsBytes(Data.AsSpan(baseOffset, length));
        return bytes;
    }

    public virtual ReadOnlySpan<byte> ReadSpan(int baseOffset, int offset, int length)
        => ReadSpan(baseOffset + offset, length);

    public virtual byte[] ReadBytes(int baseOffset, int length)
    {
        EnsurePositionInBounds(baseOffset, length);
        return Data[baseOffset..(baseOffset+length)];
    }

    public virtual byte[] ReadBytes(int baseOffset, int offset, int length)
        => ReadBytes(baseOffset + offset, length);

    public virtual int ReadPointer(int baseOffset)
    {
        EnsurePositionInBounds(baseOffset, 4);
        return ReadInt32(baseOffset);
    }

    public virtual int ReadPointer(int baseOffset, int offset)
        => ReadPointer(baseOffset + offset);

    public virtual T Read<T>(int baseOffset) where T : unmanaged
    {
        int size = Marshal.SizeOf<T>();
        EnsurePositionInBounds(baseOffset, size);

        ReadOnlySpan<byte> span = Data.AsSpan(baseOffset, size);
        return MemoryMarshal.Read<T>(span);
    }

    public virtual T Read<T>(int baseOffset, int offset) where T : unmanaged
        => Read<T>(baseOffset + offset);

    /// <summary>
    /// Fast way of accessing a pointer to a structure without copying anything.
    /// </summary>
    /// <typeparam name="T">The type of the structure</typeparam>
    /// <param name="baseOffset">The base offset</param>
    /// <returns>A readonly ref to the structure in memory</returns>
    public virtual ref readonly T ReadRef<T>(int baseOffset) where T : unmanaged
    {
        int size = Marshal.SizeOf<T>();
        EnsurePositionInBounds(baseOffset, size);

        ReadOnlySpan<byte> span = Data.AsSpan(baseOffset, size);
        return ref MemoryMarshal.AsRef<T>(span);
    }

    public virtual ref readonly T ReadRef<T>(int baseOffset, int offset) where T: unmanaged
        => ref ReadRef<T>(baseOffset + offset);

    public virtual ReadOnlySpan<T> ReadArrayRef<T>(int baseOffset, int count) where T : unmanaged
    {
        int size = Marshal.SizeOf<T>();
        int totalSize = size * count;

        EnsurePositionInBounds(baseOffset, totalSize);

        ReadOnlySpan<byte> span = Data.AsSpan(baseOffset, totalSize);
        return MemoryMarshal.Cast<byte, T>(span);
    }

    public bool IsAddressInBounds(int address, int size)
    {
        return address >= 0 && address + size < Data.Length;
    }

    protected void EnsurePositionInBounds(int baseOffset, int size)
    {
        if (!IsAddressInBounds(baseOffset, size))
            throw new IndexOutOfRangeException($"Attempted to read {size} bytes at baseOffset {baseOffset}, but ROM size is {Data.Length}.");
    }
}