namespace PokeCore.Assets.Bundles;

public sealed class AssetManifest
{
    public List<AssetBundleEntry> Entries { get; private set; }

    public AssetManifest() : this([])
    {
    }

    public AssetManifest(List<AssetBundleEntry> entries)
    {
        Entries = entries;
    }

    public void Register(string assetName, Guid assetId, AssetType assetType, uint offset, uint size)
    {
        Entries.Add(new AssetBundleEntry
        {
            Name = assetName,
            Offset = offset,
            Size = size,
            AssetId = assetId,
            AssetType = assetType,
            Dependencies = []
        });
    }

    public void WriteTo(BinaryWriter writer)
    {
        writer.Write((uint)Entries.Count);
        foreach (var entry in Entries)
        {
            writer.Write(entry.AssetId.ToString());
            writer.Write(entry.Name);
            writer.Write((byte)entry.AssetType);
            writer.Write(entry.Offset);
            writer.Write(entry.Size);
        }
    }

    public static AssetManifest ReadFrom(BinaryReader reader, long manifestLength)
    {
        long start = reader.BaseStream.Position;

        int entriesCount = (int)reader.ReadUInt32();
        List<AssetBundleEntry> entries = new(entriesCount);
        for (int i = 0; i < entriesCount; i++)
        {
            entries.Add(new AssetBundleEntry
            {
                AssetId = Guid.Parse(reader.ReadString()),
                Name = reader.ReadString(),
                AssetType = (AssetType)reader.ReadByte(),
                Offset = reader.ReadUInt32(),
                Size = reader.ReadUInt32()
            });
        }

        long read = reader.BaseStream.Position - start;
        if (read != manifestLength)
            throw new InvalidOperationException("Manifest size mismatch");

        return new AssetManifest(entries);
    }
}