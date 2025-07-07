using PokeCore.IO;
using PokeCore.IO.Services;
using YamlDotNet.Core;
using PokeCore.Common.Serializations;
using PokeTools.Assets.Services.Abstractions;
using PokeTools.Assets.Core;
using PokeCore.Assets;
using PokeCore.Common.Results;

namespace PokeTools.Assets.Services;

public sealed class DefaultMetadataStore(
    IVirtualFileSystem vfs,
    IYamlSerializer serializer,
    IAssetTypeResolver assetTypeResolver
) : IAssetMetadataStore
{
    public Task<bool> ExistsAsync(VirtualPath assetPath)
    {
        return Task.FromResult(vfs.FileExists(GetMetadataPath(assetPath)));
    }

    public async Task<Result<AssetMetadata>> GetAsync(VirtualPath assetPath)
    {
        return await ExistsAsync(assetPath) ?
            await LoadAsync(assetPath) :
            await CreateAsync(assetPath);
    }

    public async Task<Result<AssetMetadata>> LoadAsync(VirtualPath assetPath)
    {
        VirtualPath metadataPath = GetMetadataPath(assetPath);

        try
        {
            if (!vfs.FileExists(metadataPath))
                return Result.Failure($"Metadata file not found: {metadataPath}");

            using Stream metadataStream = vfs.OpenRead(metadataPath);
            if (!metadataStream.CanRead)
            {
                return Result.Failure("Metadata file is not readable");
            }

            using StreamReader reader = new(metadataStream);

            string yaml = await reader.ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(yaml))
            {
                return Result.Failure("Metadata file is empty");
            }

            var metadata = serializer.Deserialize<AssetMetadata>(yaml);
            return metadata;
        }
        catch (YamlException ex)
        {
            return Result.Failure($"Invalid metadata format: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to load metadata: {ex.Message}");
        }
    }

    public async Task<Result<AssetMetadata>> CreateAsync(VirtualPath assetPath)
    {
        AssetType assetType = assetTypeResolver.ResolveTypeFromPath(assetPath);
        if (assetType == AssetType.None)
            return Result.Failure($"Asset not supported: {assetPath}");

        var metadata = new AssetMetadata
        {
            Id = Guid.NewGuid(),
            AssetType = assetType
        };

        await SaveAsync(metadata, assetPath);

        return metadata;
    }

    public async Task<Result<Unit>> SaveAsync(AssetMetadata metadata, VirtualPath assetPath)
    {
        try
        {
            VirtualPath metadataPath = GetMetadataPath(assetPath);
            IVirtualFile metadataFile = vfs.CreateFile(metadataPath, overwrite: true);
            using Stream metadataStream = metadataFile.OpenWrite();
            using StreamWriter writer = new(metadataStream);

            string yaml = serializer.Serialize(metadata);
            await writer.WriteLineAsync(yaml);

            return Result.Success();
        }
        catch (IOException ex)
        {
            return Result.Failure($"Couldn't save metadata. {ex.Message}");
        }
        catch (YamlException ex)
        {
            return Result.Failure($"Couldn't save metadata. Serialization failed. {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Couldn't save metadata. Unknown exception catched: {ex.GetType().Name}::{ex.Message}");
        }
    }

    private static VirtualPath GetMetadataPath(VirtualPath assetPath)
    {
        return assetPath.AddExtension(".meta");
    }
}