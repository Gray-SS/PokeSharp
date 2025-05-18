using System.Diagnostics.CodeAnalysis;
using PokeSharp.Engine.Assets.Exceptions;

namespace PokeSharp.Engine.Assets;

public sealed class AssetPipeline
{
    private readonly List<IAssetImporter> _assetImporters;
    private readonly Dictionary<Type, List<IAssetProcessor>> _assetProcessors;

    private readonly Dictionary<AssetReference, object> _assetsCache;

    public AssetPipeline()
    {
        _assetImporters = new List<IAssetImporter>();
        _assetProcessors = new Dictionary<Type, List<IAssetProcessor>>();
        _assetsCache = new Dictionary<AssetReference, object>();
    }

    public void RegisterImporter(IAssetImporter importer)
    {
        _assetImporters.Add(importer);
    }

    public void RegisterProcessor(IAssetProcessor processor)
    {
        if (!_assetProcessors.TryGetValue(processor.InputType, out List<IAssetProcessor>? processors))
        {
            processors = new List<IAssetProcessor>();
            _assetProcessors[processor.InputType] = processors;
        }

        processors.Add(processor);
    }

    public T LoadAsset<T>(AssetReference assetRef) where T : class
    {
        try
        {
            if (TryGetCachedAsset(assetRef, out T? cachedAsset))
                return cachedAsset;

            IAssetImporter? importer = GetAssetImporter(assetRef);
            if (importer == null)
            {
                throw new AssetImportException(
                    $"Unable to load asset: No suitable importer found.\n"
                );
            }

            object? importedAsset = importer.Import(this, assetRef);
            if (importedAsset == null)
            {
                throw new AssetImportException(
                    $"Failed to import asset: The importer '{importer.GetType().Name}' returned null."
                );
            }

            IAssetProcessor? processor = GetAssetProcessor(importedAsset, typeof(T));
            if (processor == null)
            {
                throw new AssetProcessingException(
                    $"Unable to process asset: No processor found to convert from '{importedAsset.GetType().Name}' to '{typeof(T).Name}'."
                );
            }

            object? processedAsset = processor.Process(importedAsset);
            if (processedAsset == null)
            {
                throw new AssetProcessingException(
                    $"Failed to process asset: The processor '{processor.GetType().Name}' returned null."
                );
            }

            if (processedAsset is not T finalAsset)
            {
                throw new AssetProcessingException(
                    $"Asset processing error: Expected output type '{typeof(T).Name}', but got '{processedAsset.GetType().Name}'."
                );
            }

            _assetsCache[assetRef] = finalAsset;
            return finalAsset;
        }
        catch (AssetException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new AssetException(
                $"Unexpected error while loading asset of type '{typeof(T).Name}': {ex.Message}",
                ex
            );
        }
    }

    private IAssetProcessor? GetAssetProcessor(object importedAsset, Type outputType)
    {
        Type assetType = importedAsset.GetType();
        if (!_assetProcessors.TryGetValue(assetType, out List<IAssetProcessor>? processors))
            return null;

        foreach (IAssetProcessor processor in processors)
        {
            if (processor.OutputType == outputType)
                return processor;
        }

        return null;
    }

    private IAssetImporter? GetAssetImporter(AssetReference assetRef)
    {
        foreach (IAssetImporter importer in _assetImporters)
        {
            if (importer.CanImport(assetRef))
                return importer;
        }

        return null;
    }

    private bool TryGetCachedAsset<T>(AssetReference assetRef, [NotNullWhen(true)] out T? cachedAsset) where T : class
    {
        cachedAsset = null;

        if (!_assetsCache.TryGetValue(assetRef, out object? result))
            return false;

        cachedAsset = result as T ?? throw new InvalidOperationException(
            $"Cached asset type mismatch: expected '{typeof(T)}', but found '{result.GetType()}'."
        );

        return true;
    }
}