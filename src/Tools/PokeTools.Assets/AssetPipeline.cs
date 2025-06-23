using PokeCore.IO;
using PokeCore.IO.Services;
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
    public Task<Result> ImportAsync(IAssetImporter importer, VirtualPath path)
    {
        ThrowHelper.AssertNotNull(path);
        ThrowHelper.Assert(path.IsFile, "The specified virtual path must represent a file.");

        IVirtualFile file = vfs.GetFile(path);
        if (!file.Exists)
            return Result.FailureAsync(new Error("The specified path doesn't exists"));

        using Stream stream = file.OpenRead();
        if (!stream.CanRead)
            Result.FailureAsync(new Error("The stream is not readable"));

        Result<object> importResult = importer.Import(stream);
        if (importResult.IsFailure)
            return Result.FailureAsync(importResult.GetError());

        return Result.SuccessAsync();
    }

    public IEnumerable<IAssetImporter> FindImportersForExtension(string extension)
    {
        return services.GetServices<IAssetImporter>()
            .Where(x => x.SupportedExtensions.Any(y => string.Equals(y, extension, StringComparison.OrdinalIgnoreCase)));
    }
}