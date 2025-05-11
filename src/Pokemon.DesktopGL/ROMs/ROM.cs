using System;
using System.IO;

namespace Pokemon.DesktopGL.ROMs;

public sealed class ROM
{
    public byte[] RawData { get; }

    public ROM(byte[] rawData)
    {
        RawData = rawData;
    }

    public static ROM LoadFromPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new InvalidOperationException("Couldn't load the ROM from a null or empty path");

        if (!File.Exists(path))
            throw new InvalidOperationException($"Couln't load the ROM: The path '{path}' doesn't contain the ROM.");

        byte[] rawData = File.ReadAllBytes(path);
        return new ROM(rawData);
    }
}