using System.Text;

namespace PokeCore.Assets.Bundles;

public sealed class AssetBundleHeader
{
    public const string MagicString = "PKBUNDLE";
    public uint Version { get; set; }
    public uint AssetsCount { get; set; }
    public uint TableOffset { get; set; }
    public uint DataOffset { get; set; }
    public AssetBundleFlags Flags { get; set; }

    public void WriteTo(BinaryWriter writer)
    {
        byte[] magicBytes = Encoding.ASCII.GetBytes(MagicString);
        if (magicBytes.Length > 8)
            throw new InvalidOperationException("Magic string too long (max 8 bytes)");

        writer.Write(magicBytes);
        writer.Write(new byte[8 - magicBytes.Length]);

        writer.Write(Version);
        writer.Write(AssetsCount);
        writer.Write(TableOffset);
        writer.Write(DataOffset);
        writer.Write((int)Flags);

        long written = writer.BaseStream.Position;
        if (written > 32)
            throw new InvalidOperationException("AssetBundleHeader exceeds 32 bytes");

        writer.Write(new byte[32 - written]);
    }

    public static AssetBundleHeader ReadFrom(BinaryReader reader)
    {
        AssetBundleHeader header = new();

        string magicString = Encoding.UTF8.GetString(reader.ReadBytes(8));
        if (!MagicString.SequenceEqual(magicString))
            throw new InvalidOperationException("Invalid magic string");

        header.Version = reader.ReadUInt32();
        header.AssetsCount = reader.ReadUInt32();
        header.TableOffset = reader.ReadUInt32();
        header.DataOffset = reader.ReadUInt32();
        header.Flags = (AssetBundleFlags)reader.ReadInt32();

        long read = reader.BaseStream.Position;
        if (read > 32)
            throw new InvalidOperationException("AssetBundleHeader exceeds 32 bytes");

        reader.Read(new byte[32 - read]);
        return header;
    }
}