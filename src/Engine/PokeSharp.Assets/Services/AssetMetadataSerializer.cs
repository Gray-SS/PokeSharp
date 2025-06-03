using PokeSharp.Assets.VFS;
using PokeSharp.Core.Services;

namespace PokeSharp.Assets.Services;

public sealed class AssetMetadataSerializer : IAssetMetadataSerializer
{
    private readonly IDynamicTypeResolver _typeResolver;

    public AssetMetadataSerializer(IDynamicTypeResolver typeResolver)
    {
        _typeResolver = typeResolver;
    }

    public void Serialize(IVirtualFile vfile, AssetMetadata metadata)
    {
        using var stream = vfile.OpenWrite();
        using var writer = new BinaryWriter(stream);

        writer.Write(metadata.Id.ToString());
        writer.Write(metadata.AssetType.AssemblyQualifiedName!);
        writer.Write(metadata.HasResource);

        if (metadata.HasResource)
        {
            writer.Write(metadata.Importer!.GetType().AssemblyQualifiedName!);
            writer.Write(metadata.Processor!.GetType().AssemblyQualifiedName!);
            writer.Write(metadata.ResourcePath!.ToString());
        }

        // Write metadata dictionary
        writer.Write(metadata.Metadata.Count);
        foreach (var kvp in metadata.Metadata)
        {
            writer.Write(kvp.Key);
            WriteObject(writer, kvp.Value);
        }
    }

    public AssetMetadata Deserialize(IVirtualFile file)
    {
        byte[] rawData = file.ReadBytes();

        using var stream = new MemoryStream(rawData);
        using var reader = new BinaryReader(stream);

        var id = Guid.Parse(reader.ReadString());
        var metadata = new AssetMetadata(id);
        metadata.AssetType = _typeResolver.ResolveType(reader.ReadString())!;

        var hasResource = reader.ReadBoolean();
        if (hasResource)
        {
            metadata.Importer = _typeResolver.InstantiateFromTypeName<IAssetImporter>(reader.ReadString())!;
            metadata.Processor = _typeResolver.InstantiateFromTypeName<IAssetProcessor>(reader.ReadString())!;
            metadata.ResourcePath = VirtualPath.Parse(reader.ReadString());
        }

        var metadataCount = reader.ReadInt32();
        for (int i = 0; i < metadataCount; i++)
        {
            var key = reader.ReadString();
            metadata.Metadata[key] = ReadObject(reader);
        }

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