using System.Security.Cryptography.X509Certificates;
using PokeCore.Assets;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeTools.Assets;

public sealed class AssetPipelineProvider(
    IServiceResolver services
) : IAssetPipelineProvider
{
    public IAssetPipeline? GetPipeline(AssetType assetType)
    {
        IAssetImporter? importer = GetImporter(assetType);
        if (importer == null) return null;

        IAssetProcessor? processor = GetProcessor(assetType);
        if (processor == null) return null;

        IAssetSerializer? serializer = GetSerializer(assetType);
        if (serializer == null) return null;

        return new AssetPipeline(importer, processor, serializer);
    }

    public IAssetPipeline? GetPipeline(string extension)
    {
        IAssetImporter? importer = GetImporter(extension);
        if (importer == null) return null;

        AssetType assetType = importer.Metadata.AssetType;
        IAssetProcessor? processor = GetProcessor(assetType);
        if (processor == null) return null;

        IAssetSerializer? serializer = GetSerializer(assetType);
        if (serializer == null) return null;

        return new AssetPipeline(importer, processor, serializer);
    }

    public IAssetImporter? GetImporter(string extension)
    {
        return services.GetServices<IAssetImporter>()
                       .FirstOrDefault(x => x.Metadata.SupportedExtensions.Any(y => string.Equals(extension, y, StringComparison.OrdinalIgnoreCase)));
    }

    public IAssetImporter? GetImporter(AssetType assetType)
    {
        return services.GetServices<IAssetImporter>()
                       .FirstOrDefault(x => x.Metadata.AssetType == assetType);
    }

    public IAssetProcessor? GetProcessor(AssetType assetType)
    {
        return services.GetServices<IAssetProcessor>()
                       .FirstOrDefault(x => x.Metadata.AssetType == assetType);
    }

    public IAssetSerializer? GetSerializer(AssetType assetType)
    {
        return services.GetServices<IAssetSerializer>()
                       .FirstOrDefault(x => x.Metadata.AssetType == assetType);
    }
}