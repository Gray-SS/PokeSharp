using System.Diagnostics;
using PokeEngine.Assets.VFS;
using PokeCore.Hosting;
using PokeCore.Hosting.Logging;
using PokeCore.Hosting.Services;

namespace PokeEngine.Assets.Services;

public sealed class AssetMetadataSerializer : IAssetMetadataSerializer
{
    private readonly Logger _logger;
    private readonly IDynamicTypeResolver _typeResolver;

    public AssetMetadataSerializer(Logger logger, IDynamicTypeResolver typeResolver)
    {
        _logger = logger;
        _typeResolver = typeResolver;
    }

    public void Serialize(IVirtualFile vfile, AssetMetadata metadata)
    {
        ThrowHelper.AssertNotNull(vfile, "Asset metadata file must be not null");
        ThrowHelper.AssertNotNull(metadata, "Asset metadata must be not null");

        _logger.Trace($"Serializing asset metadata to '{vfile.Path}' ({metadata.Id})");

        using var stream = vfile.OpenWrite();
        using var writer = new BinaryWriter(stream);

        writer.Write(metadata.Id.ToString());

        // Write error
        writer.Write(metadata.IsValid);
        if (!metadata.IsValid) writer.Write(metadata.ErrorMessage!);

        writer.Write(metadata.AssetType != null);
        if (metadata.AssetType != null) writer.Write(metadata.AssetType.AssemblyQualifiedName!);

        writer.Write(metadata.Importer != null);
        if (metadata.Importer != null) writer.Write(metadata.Importer.GetType().AssemblyQualifiedName!);

        writer.Write(metadata.ResourcePath != null);
        if (metadata.ResourcePath != null) writer.Write(metadata.ResourcePath.Uri);

        // Write metadata dictionary
        writer.Write(metadata.Metadata.Count);
        foreach (var kvp in metadata.Metadata)
        {
            writer.Write(kvp.Key);
            WriteObject(writer, kvp.Value);
        }

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

        bool isValid = reader.ReadBoolean();
        if (!isValid) metadata.ErrorMessage = reader.ReadString();

        bool hasAssetType = reader.ReadBoolean();
        if (hasAssetType) metadata.AssetType = _typeResolver.ResolveType(reader.ReadString());

        bool hasImporter = reader.ReadBoolean();
        if (hasImporter) metadata.Importer = _typeResolver.InstantiateFromTypeName<IAssetImporter>(reader.ReadString());

        bool hasResPath = reader.ReadBoolean();
        if (hasResPath) metadata.ResourcePath = VirtualPath.Parse(reader.ReadString());

        var metadataCount = reader.ReadInt32();
        for (int i = 0; i < metadataCount; i++)
        {
            var key = reader.ReadString();
            metadata.Metadata[key] = ReadObject(reader);
        }

        _logger.Trace($"Asset metadata deserialized ({metadata.Id}).");
        return metadata;
    }

    private static void WriteObject(BinaryWriter writer, object value)
    {
        switch (value)
        {
            case int i:
                writer.Write("int");
                writer.Write(i);
                break;
            case float f:
                writer.Write("float");
                writer.Write(f);
                break;
            case bool b:
                writer.Write("bool");
                writer.Write(b);
                break;
            case string s:
                writer.Write("string");
                writer.Write(s);
                break;
            default:
                throw new NotSupportedException($"Unsupported metadata type: {value.GetType()}");
        }
    }

    private static object ReadObject(BinaryReader reader)
    {
        var type = reader.ReadString();
        return type switch
        {
            "int" => reader.ReadInt32(),
            "float" => reader.ReadSingle(),
            "bool" => reader.ReadBoolean(),
            "string" => reader.ReadString(),
            _ => throw new NotSupportedException($"Unsupported metadata type tag: {type}")
        };
    }
}