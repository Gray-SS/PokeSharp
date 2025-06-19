using PokeCore.IO;
using PokeCore.Logging;
using PokeCore.Diagnostics;
using PokeTools.Assets;
using PokeLab.Domain.Assets;

namespace PokeLab.Application.Assets;

public sealed class AssetMetadataSerializer : IAssetMetadataSerializer
{
    private readonly Logger _logger;

    public AssetMetadataSerializer(Logger<AssetMetadataSerializer> logger)
    {
        _logger = logger;
    }

    public void Serialize(IVirtualFile file, AssetMetadata metadata)
    {
        ThrowHelper.AssertNotNull(file, "Asset metadata file must be not null");
        ThrowHelper.AssertNotNull(metadata, "Asset metadata must be not null");

        _logger.Trace($"Serializing asset metadata to '{file.Path}' ({metadata.Id})");

        using var stream = file.OpenWrite();
        using var writer = new BinaryWriter(stream);

        writer.Write(metadata.Id.ToString());

        writer.Write((byte)metadata.AssetType);

        writer.Write(metadata.ResourcePath != null);
        if (metadata.ResourcePath != null) writer.Write(metadata.ResourcePath.Uri);

        _logger.Trace($"Asset metadata serialized ({metadata.Id})");
    }

    public AssetMetadata Deserialize(IVirtualFile file)
    {
        ThrowHelper.AssertNotNull(file, "Metadata file must be not null");

        _logger.Trace($"Deserializing asset metadata from '{file.Path}'");

        byte[] rawData = file.ReadBytes();

        using var stream = new MemoryStream(rawData);
        using var reader = new BinaryReader(stream);

        var metadata = new AssetMetadata();
        metadata.Id = Guid.Parse(reader.ReadString());

        metadata.AssetType = (AssetType)reader.ReadByte();

        bool hasResPath = reader.ReadBoolean();
        if (hasResPath) metadata.ResourcePath = VirtualPath.Parse(reader.ReadString());

        _logger.Trace($"Asset metadata deserialized ({metadata.Id}).");
        return metadata;
    }
}