namespace PokeCore.Assets.Bundles;

public sealed class AssetManifest
{
    public Dictionary<Guid, AssetBundleEntry> Entries { get; private set; }

    public AssetManifest() : this([])
    {
    }

    public AssetManifest(List<AssetBundleEntry> entries)
    {
        Entries = entries.ToDictionary(x => x.AssetId);
    }

    public void Register(string assetName, Guid assetId, AssetType assetType, uint offset, uint size, List<Guid> dependencies)
    {
        Entries.Add(assetId, new AssetBundleEntry
        {
            Name = assetName,
            Offset = offset,
            Size = size,
            AssetId = assetId,
            AssetType = assetType,
            Dependencies = dependencies
        });
    }

    public AssetBundleEntry? GetEntry(Guid id)
        => Entries.TryGetValue(id, out AssetBundleEntry? entry) ? entry : null;

    public void WriteTo(BinaryWriter writer)
    {
        writer.Write((uint)Entries.Count);
        foreach (var entry in Entries.Values)
        {
            writer.Write(entry.AssetId.ToString());
            writer.Write(entry.Name);
            writer.Write((byte)entry.AssetType);
            writer.Write(entry.Offset);
            writer.Write(entry.Size);

            writer.Write(entry.Dependencies.Count);
            foreach (Guid dependency in entry.Dependencies)
                writer.Write(dependency.ToString());
        }
    }

    public static AssetManifest ReadFrom(BinaryReader reader, long manifestLength)
    {
        long start = reader.BaseStream.Position;

        int entriesCount = (int)reader.ReadUInt32();
        List<AssetBundleEntry> entries = new(entriesCount);
        for (int i = 0; i < entriesCount; i++)
        {
            var assetId = Guid.Parse(reader.ReadString());
            var assetName = reader.ReadString();
            var assetType = (AssetType)reader.ReadByte();
            var offset = reader.ReadUInt32();
            var size = reader.ReadUInt32();

            var dependenciesCount = reader.ReadInt32();
            var dependencies = new List<Guid>(dependenciesCount);
            for (int j = 0; j < dependenciesCount; j++)
                dependencies.Add(Guid.Parse(reader.ReadString()));

            entries.Add(new AssetBundleEntry
            {
                AssetId = assetId,
                Name = assetName,
                AssetType = assetType,
                Offset = offset,
                Size = size,
                Dependencies = dependencies
            });
        }

        long read = reader.BaseStream.Position - start;
        if (read != manifestLength)
            throw new InvalidOperationException("Manifest size mismatch");

        return new AssetManifest(entries);
    }
}