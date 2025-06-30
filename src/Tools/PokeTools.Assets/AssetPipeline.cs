using PokeCore.IO;
using PokeCore.IO.Services;
using PokeCore.Assets;
using PokeCore.Common;
using PokeCore.Diagnostics;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeTools.Assets;

public sealed class AssetPipeline(
    IVirtualFileSystem vfs,
    IServiceResolver services
) : IAssetPipeline
{
    public async Task<Result> BuildAsync(VirtualPath inputPath, VirtualPath outputPath)
    {
        ThrowHelper.AssertNotNull(inputPath);
        ThrowHelper.AssertNotNull(outputPath);

        IAssetImporter? importer = services.GetServices<IAssetImporter>()
                                           .FirstOrDefault(x => x.Metadata.SupportedExtensions.Any(y => string.Equals(inputPath.Extension, y, StringComparison.OrdinalIgnoreCase)));

        if (importer == null)
            return Result.Failure(new Error($"No importer found for '{inputPath.Extension}' files."));

        IAssetProcessor? processor = services.GetServices<IAssetProcessor>()
                                             .FirstOrDefault(x => x.Metadata.AssetType == importer.Metadata.AssetType);

        if (processor == null)
            return Result.Failure(new Error($"No processor found for '{importer.Metadata.AssetType}'"));

        IAssetCompiler? compiler = services.GetServices<IAssetCompiler>()
                                   .FirstOrDefault(x => x.Metadata.AssetType == importer.Metadata.AssetType);

        if (compiler == null)
            return Result.Failure(new Error($"No compiler found to compile assets of type '{importer.Metadata.AssetType}'"));

        object? rawAsset;
        using (Stream inputStream = vfs.OpenRead(inputPath))
        {
            Result<object> importResult = importer.Import(inputStream);
            if (!importResult.TryGetValue(out rawAsset))
                return Result.Failure(new Error($"Import failed. {importResult.GetError().Message}"));
        }

        Guid assetId = Guid.NewGuid();
        Result<IAsset> processResult = processor.Process(assetId, rawAsset);
        if (!processResult.TryGetValue(out IAsset? asset))
            return Result.Failure(new Error($"Process failed. {processResult.GetError().Message}"));

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

    // public Result BuildBundle(VirtualPath dirPath, string bundleName)
    // {
    //     var assets = new List<IAsset>();
    //     foreach (IVirtualFile file in vfs.GetFilesRecursive(dirPath))
    //     {
    //         VirtualPath inputPath = file.Path;
    //         IAssetBuilder? builder = services.GetServices<IAssetBuilder>()
    //                                          .FirstOrDefault(x => x.CanBuild(inputPath.Extension));

    //         if (builder == null)
    //             continue;

    //         Result<IAsset> buildResult = builder.Build(inputPath);
    //         if (!buildResult.TryGetValue(out IAsset? asset))
    //             return Result.Failure(new Error($"Build failed. {buildResult.GetError().Message}"));

    //         assets.Add(asset);
    //     }

    //     VirtualPath outputPath = VirtualPathHelper.ResolvePhysicalPath($"./{bundleName}.pak");
    //     IVirtualFile outputFile = vfs.CreateFile(outputPath, true);
    //     using Stream stream = outputFile.OpenWrite();
    //     using BinaryWriter writer = new(stream);

    //     writer.Write(assets.Count);
    //     long currentOffset = sizeof(int) + Marshal.SizeOf<AssetBundleEntryHeader>() * assets.Count;
    //     for (int i = 0; i < assets.Count; i++)
    //     {
    //         IAsset asset = assets[i];

    //         stream.Position = sizeof(int) + i * Marshal.SizeOf<AssetBundleEntryHeader>();
    //         writer.Write(asset.Id.ToString());
    //         writer.Write(currentOffset);

    //         stream.Position = currentOffset;
    //         IAssetCompiler? compiler = services.GetServices<IAssetCompiler>()
    //                                            .FirstOrDefault(x => x.Metadata.AssetType == asset.AssetType);

    //         if (compiler == null)
    //             return Result.Failure(new Error($"No compiler found to compile assets of type '{asset.AssetType}'"));

    //         Result compileResult = compiler.Compile(asset, writer);
    //         if (compileResult.TryGetError(out Error? error))
    //             return Result.Failure(error);

    //         currentOffset = stream.Position;
    //     }

    //     return Result.Success();
    // }
}