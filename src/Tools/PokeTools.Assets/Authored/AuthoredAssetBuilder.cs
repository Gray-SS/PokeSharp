using PokeCore.IO;
using PokeCore.IO.Services;
using PokeCore.Assets;
using PokeCore.Common;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;
using YamlDotNet.Serialization;

namespace PokeTools.Assets.Authored;

public sealed class AuthoredAssetBuilder(
    IServiceResolver services,
    IVirtualFileSystem vfs
) : IAssetBuilder
{
    public bool CanBuild(string extension)
    {
        return FindLoader(extension) != null;
    }

    public Result<IAsset> Build(VirtualPath path)
    {
        IAssetLoader? loader = FindLoader(path.Extension);
        if (loader == null)
            return Result<IAsset>.Failure(new Error($"No loader found to load internal asset of type '{path.Extension}'"));

        using Stream stream = vfs.OpenRead(path);
        using StreamReader reader = new(stream);

        if (stream.CanSeek)
            stream.Seek(0, SeekOrigin.Begin);

        string input = reader.ReadToEnd();

        object? descriptor;
        try
        {
            descriptor = new Deserializer().Deserialize(input, loader.DescriptorType);
            if (descriptor == null)
                return Result<IAsset>.Failure(new Error($"Couldn't deserialize the descriptor, make sure the file is correctly formated."));
        }
        catch (Exception)
        {
            return Result<IAsset>.Failure(new Error($"Couldn't deserialize the descriptor."));
        }

        Guid assetId = Guid.NewGuid();
        IAsset asset = loader.Load(assetId, descriptor);

        return Result<IAsset>.Success(asset);
    }

    private IAssetLoader? FindLoader(string extension)
    {
        return services.GetServices<IAssetLoader>()
                       .FirstOrDefault(x => string.Equals(x.Metadata.Extension, extension, StringComparison.OrdinalIgnoreCase));
    }
}
