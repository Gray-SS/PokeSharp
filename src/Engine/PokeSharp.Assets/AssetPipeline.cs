using System.Diagnostics.CodeAnalysis;
using PokeSharp.Assets.Exceptions;
using PokeSharp.Core.Exceptions;
using PokeSharp.Core.Services;

namespace PokeSharp.Assets;

public sealed class AssetPipeline
{
    public IReadOnlyCollection<object> LoadedAssets => _cachedAssets.Values;

    private readonly IAssetImporter[] _importers;
    private readonly IAssetProcessor[] _processors;

    private readonly IReflectionManager _reflectionManager;
    private readonly Dictionary<string, object> _cachedAssets;

    public AssetPipeline(IReflectionManager reflectionManager)
    {
        _reflectionManager = reflectionManager;

        _importers = LoadImporters();
        _processors = LoadProcessors();

        _cachedAssets = new Dictionary<string, object>();
    }

    private IAssetImporter[] LoadImporters()
    {
        return _reflectionManager.InstantiateClassesOfType<IAssetImporter>();
    }

    private IAssetProcessor[] LoadProcessors()
    {
        return _reflectionManager.InstantiateClassesOfType<IAssetProcessor>();
    }

    public object Load(string path)
    {
        if (TryGetCachedAsset(path, out object? cachedAsset))
        {
            return cachedAsset;
        }

        string ext = Path.GetExtension(path);

        IAssetImporter importer = this.GetAssetImporterFromExtension(ext);
        object importedAsset = ImportAssetInternal(importer, path);

        IAssetProcessor processor = this.GetDefaultProcessorFromImporter(importer, importedAsset);
        object loadedAsset = ProcessAssetInternal(processor, importedAsset, path);

        _cachedAssets[path] = loadedAsset;
        return loadedAsset;
    }

    public T Load<T>(string path) where T : class
    {
        object loadedAsset = Load(path);
        if (loadedAsset is not T asset)
        {
            throw new AssetPipelineException($"""
                The processed asset is of type '{loadedAsset.GetType().Name}', but the pipeline expected '{typeof(T).Name}'.
                Ensure the processor output type matches the requested asset type.
            """);
        }

        return asset;
    }

    private IAssetImporter GetAssetImporterFromExtension(string ext)
    {
        IAssetImporter? importer = _importers.FirstOrDefault(x => x.CanImport(ext));
        if (importer is null)
        {
            throw new AssetPipelineException($"""
                No importer found for file extension '{ext}'.
                This asset type may not be supported. Please verify the extension and ensure a matching importer is registered.
            """);
        }

        return importer;
    }

    private IAssetProcessor GetDefaultProcessorFromImporter(IAssetImporter importer, object rawAsset)
    {
        IAssetProcessor? processor = _processors.FirstOrDefault(x => importer.ProcessorType == x.GetType());
        if (processor == null)
        {
            throw new AssetPipelineException($"""
                The default processor '{importer.ProcessorType.Name}' for importer '{importer.GetType().Name}' was not found for asset of type '{rawAsset.GetType().Name}'.
                Make sure the module associated with the processor is registered.
            """);
        }

        return processor;
    }

    private object ImportAssetInternal(IAssetImporter importer, string path)
    {
        object? rawAsset;
        try
        {
            rawAsset = importer.Import(path);
        }
        catch (AssetImporterException)
        {
            throw; // Re-throw user-thrown asset-specific exception.
        }
        catch (AssetProcessorException)
        {
            throw new AssetPipelineException($"""
                A processor exception was thrown during import by '{importer.GetType().Name}'.
                This indicates a misuse of exception types. Importers must throw {nameof(AssetImporterException)} instead.
            """);
        }
        catch (EngineException)
        {
            throw new AssetPipelineException($"""
                An {nameof(EngineException)} was thrown during asset import from '{path}' using '{importer.GetType().Name}'.
                Please use the appropriate {nameof(AssetImporterException)} instead, to indicate an importer-level error.
            """);
        }
        catch (Exception ex)
        {
            throw new AssetPipelineException($"""
                An unexpected error occurred while importing asset from '{path}' using '{importer.GetType().Name}'.
                Please ensure the file format is valid and that the importer handles it correctly.
            """, ex);
        }

        if (rawAsset is null)
        {
            throw new AssetPipelineException($"""
                Importer '{importer.GetType().Name}' returned null for asset at '{path}'.
                This indicates an internal failure. Ensure the file is valid and supported by the importer.
            """);
        }

        return rawAsset;
    }

    private object ProcessAssetInternal(IAssetProcessor processor, object rawAsset, string path)
    {
        object? asset;
        try
        {
            asset = processor.Process(rawAsset);
        }
        catch (AssetProcessorException)
        {
            throw;
        }
        catch (AssetImporterException)
        {
            throw new AssetPipelineException($"""
                An import exception was thrown during processing by '{processor.GetType().Name}'.
                This is likely a misuse of exception types. Processors should throw {nameof(AssetProcessorException)} instead.
            """);
        }
        catch (EngineException)
        {
            throw new AssetPipelineException($"""
                An {nameof(EngineException)} was thrown while processing asset at '{path}' using '{processor.GetType().Name}'.
                Processors must throw {nameof(AssetProcessorException)} to indicate asset processing errors.
            """);
        }
        catch (Exception ex)
        {
            throw new AssetPipelineException($"""
                An unexpected error occurred while processing asset at '{path}' using '{processor.GetType().Name}'.
                Verify the processor logic and that the input asset is valid and correctly formatted.
            """, ex);
        }

        if (asset is null)
        {
            throw new AssetPipelineException($"""
                Processor '{processor.GetType().Name}' returned null for asset at '{path}'.
                This likely indicates an unhandled error during processing.
            """);
        }

        return asset;
    }

    private bool TryGetCachedAsset(string path, [NotNullWhen(true)] out object? cachedAsset)
    {
        cachedAsset = null;

        if (!_cachedAssets.TryGetValue(path, out object? asset))
        {
            return false;
        }

        cachedAsset = asset;
        return true;
    }
}