using PokeCore.IO;
using PokeCore.IO.Services;
using PokeCore.Common;
using PokeCore.Diagnostics;
using YamlDotNet.Serialization;
using PokeCore.Assets;
using PokeCore.Assets.Bundles;

namespace PokeTools.Assets;

public sealed class AssetBuildServices(
    IVirtualFileSystem vfs,
    IAssetPipelineProvider pipelineProvider
) : IAssetBuildServices
{
    public async Task<Result> BuildAsync(VirtualPath inputPath, VirtualPath outputPath)
    {
        ThrowHelper.AssertNotNull(inputPath);
        ThrowHelper.AssertNotNull(outputPath);

        IAssetPipeline? pipeline = pipelineProvider.GetPipeline(inputPath.Extension);
        if (pipeline == null)
            return Result.Failure(new($"No importer, processor or compiler found for '{inputPath.Extension}' files."));

        AssetMetadata metadata;
        VirtualPath metadataPath = inputPath.AddExtension(".meta");
        if (vfs.FileExists(metadataPath))
        {
            using Stream metadataStream = vfs.OpenRead(metadataPath);
            using StreamReader reader = new(metadataStream);

            string yaml = await reader.ReadToEndAsync();
            metadata = new Deserializer().Deserialize<AssetMetadata>(yaml);
        }
        else
        {
            metadata = new AssetMetadata
            {
                Id = Guid.NewGuid(),
                AssetType = pipeline.AssetType
            };

            IVirtualFile metadataFile = vfs.CreateFile(metadataPath, overwrite: false);
            using Stream metadataStream = metadataFile.OpenWrite();
            using StreamWriter writer = new(metadataStream);

            string yaml = new Serializer().Serialize(metadata);
            await writer.WriteLineAsync(yaml);
        }

        using Stream inputStream = vfs.OpenRead(inputPath);

        IVirtualFile outputFile = vfs.CreateFile(outputPath, overwrite: true);
        using Stream outputStream = outputFile.OpenWrite();

        return await pipeline.BuildAsync(metadata, inputStream, outputStream);
    }

    public Result BuildManifest(VirtualPath dirPath)
    {
        var manifest = new AssetManifest();
        using var dataStream = new MemoryStream();

        foreach (IVirtualFile file in vfs.GetFilesRecursive(dirPath))
        {
            if (file.Path.Extension != ".asset")
                continue;

            using var stream = file.OpenRead();
            using var reader = new BinaryReader(stream);

            Guid assetId = Guid.Parse(reader.ReadString());
            AssetType assetType = (AssetType)reader.ReadByte();

            uint remainingLength = (uint)(stream.Length - stream.Position);
            byte[] buffer = reader.ReadBytes((int)remainingLength);

            uint offset = (uint)dataStream.Position;
            dataStream.Write(buffer, 0, buffer.Length);

            manifest.Register(file.NameWithoutExtension, assetId, assetType, offset, remainingLength);
        }

        using var manifestStream = new MemoryStream();
        var manifestWriter = new BinaryWriter(manifestStream);
        manifest.WriteTo(manifestWriter);
        manifestWriter.Flush();

        var header = new AssetBundleHeader
        {
            Version = 1,
            AssetsCount = (uint)manifest.Entries.Count,
            TableOffset = 32,
            DataOffset = (uint)(32 + manifestStream.Length),
            Flags = AssetBundleFlags.None,
        };

        VirtualPath parentPath = dirPath.GetParent();
        VirtualPath bundlePath = parentPath.Combine("mygame.bundle");
        IVirtualFile bundleFile = vfs.CreateFile(bundlePath, overwrite: true);
        using var bundleStream = bundleFile.OpenWrite();
        using var bundleWriter = new BinaryWriter(bundleStream);

        header.WriteTo(bundleWriter);

        manifestStream.Position = 0;
        manifestStream.CopyTo(bundleStream);

        dataStream.Position = 0;
        dataStream.CopyTo(bundleStream);

        manifestWriter.Dispose();
        return Result.Success();
    }
}