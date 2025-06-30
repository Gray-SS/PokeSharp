using PokeCore.IO;
using PokeCore.IO.Services;
using PokeCore.Common;
using PokeCore.Diagnostics;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;
using PokeCore.Assets;
using PokeTools.Assets.Authored;
using YamlDotNet.Serialization;
using PokeCore.Assets.Bundles;
using System.Runtime.InteropServices;

namespace PokeTools.Assets;

public sealed class AssetPipeline(
    IVirtualFileSystem vfs,
    IServiceResolver services
) : IAssetPipeline
{
    public async Task<Result> NewAsync(AssetType assetType, VirtualPath outputPath)
    {
        ThrowHelper.Assert(assetType != AssetType.None, "Asset type cannot be none");
        ThrowHelper.AssertNotNull(outputPath);

        IAssetLoader? loader = services.GetServices<IAssetLoader>()
                                       .FirstOrDefault(x => x.Metadata.AssetType == assetType);

        if (loader == null)
            return Result.Failure(new Error("This type of asset is not supported or cannot be instantiated."));

        object descriptor = Activator.CreateInstance(loader.DescriptorType)!;
        string yaml = new Serializer().Serialize(descriptor);

        IVirtualFile outputFile = vfs.CreateFile(outputPath, overwrite: true);

        using Stream stream = outputFile.OpenWrite();
        using StreamWriter writer = new(stream);
        await writer.WriteLineAsync(yaml);

        return Result.Success();
    }

    public async Task<Result> BuildAsync(VirtualPath inputPath, VirtualPath outputPath)
    {
        ThrowHelper.AssertNotNull(inputPath);
        ThrowHelper.AssertNotNull(outputPath);

        IAssetBuilder? builder = services.GetServices<IAssetBuilder>()
                                         .FirstOrDefault(x => x.CanBuild(inputPath.Extension));

        if (builder == null)
            return Result.Failure(new Error("Asset not recognized."));

        Result<IAsset> buildResult = builder.Build(inputPath);
        if (!buildResult.TryGetValue(out IAsset? asset))
            return Result.Failure(new Error($"Build failed. {buildResult.GetError().Message}"));

        IAssetCompiler? compiler = services.GetServices<IAssetCompiler>()
                                           .FirstOrDefault(x => x.Metadata.AssetType == asset.AssetType);

        if (compiler == null)
            return Result.Failure(new Error($"No compiler found to compile assets of type '{asset.AssetType}'"));

        IVirtualFile outputFile = vfs.CreateFile(outputPath, overwrite: true);

        using (Stream stream = outputFile.OpenWrite())
        {
            if (!stream.CanWrite)
                return Result.Failure(new Error("The stream is not writeable"));

            BinaryWriter writer = new(stream);
            writer.Write(asset.Id.ToString());
            writer.Write((byte)asset.AssetType);

            compiler.Compile(asset, writer);

            writer.Flush();
            await stream.FlushAsync();

            if (stream.Position == 0)
                return Result.Failure(new Error($"No bytes written to '{outputPath}'"));

            await writer.DisposeAsync();
        }

        return Result.Success();
    }

    public Result BuildBundle(VirtualPath dirPath, string bundleName)
    {
        var assets = new List<IAsset>();
        foreach (IVirtualFile file in vfs.GetFilesRecursive(dirPath))
        {
            VirtualPath inputPath = file.Path;
            IAssetBuilder? builder = services.GetServices<IAssetBuilder>()
                                             .FirstOrDefault(x => x.CanBuild(inputPath.Extension));

            if (builder == null)
                continue;

            Result<IAsset> buildResult = builder.Build(inputPath);
            if (!buildResult.TryGetValue(out IAsset? asset))
                return Result.Failure(new Error($"Build failed. {buildResult.GetError().Message}"));

            assets.Add(asset);
        }

        VirtualPath outputPath = VirtualPathHelper.ResolvePhysicalPath($"./{bundleName}.pak");
        IVirtualFile outputFile = vfs.CreateFile(outputPath, true);
        using Stream stream = outputFile.OpenWrite();
        using BinaryWriter writer = new(stream);

        writer.Write(assets.Count);
        long currentOffset = sizeof(int) + Marshal.SizeOf<AssetBundleEntryHeader>() * assets.Count;
        for (int i = 0; i < assets.Count; i++)
        {
            IAsset asset = assets[i];

            stream.Position = sizeof(int) + i * Marshal.SizeOf<AssetBundleEntryHeader>();
            writer.Write(asset.Id.ToString());
            writer.Write(currentOffset);

            stream.Position = currentOffset;
            IAssetCompiler? compiler = services.GetServices<IAssetCompiler>()
                                               .FirstOrDefault(x => x.Metadata.AssetType == asset.AssetType);

            if (compiler == null)
                return Result.Failure(new Error($"No compiler found to compile assets of type '{asset.AssetType}'"));

            Result compileResult = compiler.Compile(asset, writer);
            if (compileResult.TryGetError(out Error? error))
                return Result.Failure(error);

            currentOffset = stream.Position;
        }

        return Result.Success();
    }
}