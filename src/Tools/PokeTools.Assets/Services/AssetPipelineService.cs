using PokeCore.IO;
using PokeCore.IO.Services;
using PokeCore.Assets;
using PokeCore.Assets.Bundles;
using PokeCore.Common;
using PokeCore.Diagnostics;
using PokeTools.Assets.Services.Abstractions;
using PokeTools.Assets.Models;
using PokeTools.Assets.Core;
using PokeCore.Common.Results;
using PokeCore.Common.Results.Extensions;

namespace PokeTools.Assets.Services;

public sealed class AssetPipelineService(
    IVirtualFileSystem vfs,
    IAssetMetadataStore metadataStore,
    IAssetPipelineProvider pipelineProvider,
    IAssetPipelineExecutor pipelineExecutor
) : IAssetPipelineService
{
    public async Task<Result<Unit>> BuildAsync(VirtualPath inputPath, VirtualPath outputPath)
    {
        ThrowHelper.AssertNotNull(inputPath);
        ThrowHelper.AssertNotNull(outputPath);

        Result<AssetMetadata> getMetadataResult = await metadataStore.GetAsync(inputPath);
        if (getMetadataResult.IsFailure)
            return Result.Failure(getMetadataResult.Error);

        AssetMetadata metadata = getMetadataResult.GetValue();
        using Stream inputStream = vfs.OpenRead(inputPath);

        IVirtualFile outputFile = vfs.CreateFile(outputPath, overwrite: true);
        using Stream outputStream = outputFile.OpenWrite();

        var getPipelineResult = pipelineProvider.GetPipeline(metadata.AssetType);
        if (getPipelineResult == null)
            return Result.Failure(new($"No pipeline found for assets of type '{metadata.AssetType}'"));

        AssetPipelineDefinition pipelineDefinition = getPipelineResult.GetValue();

        var executionResult = await pipelineExecutor.ExecuteAsync(pipelineDefinition, metadata, inputStream, outputStream);
        return executionResult.ToUnit();
    }

    public Result<Unit> BuildBundle(VirtualPath dirPath)
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

            int dependenciesCount = reader.ReadInt32();
            var dependencies = new List<Guid>(dependenciesCount);
            for (int i = 0; i < dependenciesCount; i++)
            {
                Guid depsId = Guid.Parse(reader.ReadString());
                dependencies.Add(depsId);
            }

            uint remainingLength = (uint)(stream.Length - stream.Position);
            byte[] buffer = reader.ReadBytes((int)remainingLength);

            uint offset = (uint)dataStream.Position;
            dataStream.Write(buffer, 0, buffer.Length);

            manifest.Register(file.NameWithoutExtension, assetId, assetType, offset, remainingLength, dependencies);
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