using PokeCore.IO;
using PokeCore.IO.Services;
using PokeCore.Assets;
using PokeCore.Common;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeTools.Assets.External;

public sealed class ExternalAssetBuilder(
    IServiceResolver services,
    IVirtualFileSystem vfs
) : IAssetBuilder
{
    public bool CanBuild(string extension)
    {
        return FindImporter(extension) != null;
    }

    public Result<IAsset> Build(VirtualPath path)
    {
        IAssetImporter? importer = FindImporter(path.Extension);
        if (importer == null)
            return Result<IAsset>.Failure(new Error($"No importer found to import resources with extension '{path.Extension}'"));

        IAssetProcessor? processor = FindProcessor(importer.Metadata.AssetType);
        if (processor == null)
            return Result<IAsset>.Failure(new Error($"No processor found to process assets of type '{importer.Metadata.AssetType}'"));

        Result<object> importResult;
        using (Stream stream = vfs.OpenRead(path))
        {
            if (!stream.CanRead)
                return Result<IAsset>.Failure(new Error("The stream is not readable"));

            importResult = importer.Import(stream);
        }

        if (!importResult.TryGetValue(out object? rawAsset))
            return Result<IAsset>.Failure(importResult.GetError());

        Guid assetId = Guid.NewGuid();
        Result<IAsset> processResult = processor.Process(assetId, rawAsset);

        if (!processResult.TryGetValue(out IAsset? asset))
            return Result<IAsset>.Failure(processResult.GetError());

        return Result<IAsset>.Success(asset);
    }

    private IAssetImporter? FindImporter(string extension)
    {
        return services.GetServices<IAssetImporter>()
                       .FirstOrDefault(x => x.Metadata.SupportedExtensions.Any(y => string.Equals(y, extension, StringComparison.OrdinalIgnoreCase)));
    }

    private IAssetProcessor? FindProcessor(AssetType assetType)
    {
        return services.GetServices<IAssetProcessor>()
                       .FirstOrDefault(x => x.Metadata.AssetType == assetType);
    }
}