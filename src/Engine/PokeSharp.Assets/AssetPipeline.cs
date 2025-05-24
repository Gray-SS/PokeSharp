using System.Diagnostics.CodeAnalysis;
using PokeSharp.Assets.Exceptions;
using PokeSharp.Core.Exceptions;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Services;

namespace PokeSharp.Assets;

public sealed class AssetPipeline
{
    public IReadOnlyCollection<object> LoadedAssets => _cachedAssets.Values;

    private readonly IAssetImporter[] _importers;
    private readonly IAssetProcessor[] _processors;

    private readonly ILogger _logger;
    private readonly IReflectionManager _reflectionManager;
    private readonly Dictionary<string, object> _cachedAssets;

    public AssetPipeline(IReflectionManager reflectionManager, ILogger logger)
    {
        _logger = logger;
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

    public object? Load(string path)
    {
        try
        {
            _logger.Debug($"Loading asset: '{path}'");

            if (string.IsNullOrEmpty(path))
            {
                throw new AssetPipelineException("Asset path cannot be null or empty");
            }

            string fullPath = Path.GetFullPath(path);
            _logger.Trace($"Resolved full path: '{fullPath}'");

            if (!File.Exists(fullPath))
            {
                throw new AssetPipelineException($"File not found: '{fullPath}'");
            }

            if (TryGetCachedAsset(fullPath, out object? cachedAsset))
            {
                _logger.Trace($"Using cached asset: '{path}'");
                return cachedAsset;
            }

            string ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext))
            {
                throw new AssetPipelineException($"File extension required for asset: '{path}'");
            }

            _logger.Debug($"Processing '{ext}' file: '{path}'");

            var importer = GetAssetImporterFromExtension(ext);
            var importedAsset = ImportAssetInternal(importer, path);

            var processor = GetDefaultProcessorFromImporter(importer, importedAsset);
            var loadedAsset = ProcessAssetInternal(processor, importedAsset, path);

            _cachedAssets[fullPath] = loadedAsset;
            _logger.Debug($"Asset cached with key: {fullPath}");
            _logger.Info($"Asset loaded successfully: '{path}' -> {loadedAsset.GetType().Name}");

            return loadedAsset;
        }
        catch (AssetProcessorException ex)
        {
            _logger.Error($"Asset processing failed for '{path}': {ex.Message}");
        }
        catch (AssetImporterException ex)
        {
            _logger.Error($"Asset import failed for '{path}': {ex.Message}");
        }
        catch (AssetPipelineException ex)
        {
            _logger.Error($"Asset pipeline error for '{path}': {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Unexpected error loading '{path}': {ex.GetType().Name} - {ex.Message}");
            _logger.Debug($"Stack trace: {ex.StackTrace ?? "Not available"}");
        }

        return null;
    }

    public T? Load<T>(string path) where T : class
    {
        try
        {
            var loadedAsset = Load(path);
            if (loadedAsset == null)
            {
                return null;
            }

            if (loadedAsset is not T asset)
            {
                throw new AssetPipelineException($"Type mismatch: expected '{typeof(T).Name}', got '{loadedAsset.GetType().Name}' for asset '{path}'");
            }

            return asset;
        }
        catch (AssetPipelineException ex)
        {
            _logger.Error($"Generic load failed for '{path}': {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Unexpected error in generic load for '{path}': {ex.GetType().Name} - {ex.Message}");
            _logger.Debug($"Stack trace: {ex.StackTrace ?? "Not available"}");
        }

        return null;
    }

    private IAssetImporter GetAssetImporterFromExtension(string ext)
    {
        _logger.Trace($"Finding importer for extension: '{ext}'");

        var importer = _importers.FirstOrDefault(x => x.CanImport(ext));
        if (importer is null)
        {
            throw new AssetPipelineException($"No importer found for extension '{ext}' - asset type may not be supported");
        }

        _logger.Trace($"Using importer: {importer.GetType().Name}");
        return importer;
    }

    private IAssetProcessor GetDefaultProcessorFromImporter(IAssetImporter importer, object rawAsset)
    {
        _logger.Trace($"Finding processor '{importer.ProcessorType.Name}' for importer '{importer.GetType().Name}'");

        var processor = _processors.FirstOrDefault(x => importer.ProcessorType == x.GetType());
        if (processor == null)
        {
            throw new AssetPipelineException($"Processor '{importer.ProcessorType.Name}' not found for importer '{importer.GetType().Name}' - ensure the module is registered");
        }

        _logger.Trace($"Using processor: {processor.GetType().Name} ({rawAsset.GetType().Name} -> {processor.OutputType.Name})");
        return processor;
    }

    private object ImportAssetInternal(IAssetImporter importer, string path)
    {
        try
        {
            _logger.Trace($"Importing with {importer.GetType().Name}");

            var rawAsset = importer.Import(path);
            if (rawAsset is null)
            {
                throw new AssetPipelineException($"Importer '{importer.GetType().Name}' returned null for '{path}' - file may be corrupted or unsupported");
            }

            _logger.Trace($"Import successful: {rawAsset.GetType().Name}");
            return rawAsset;
        }
        catch (AssetProcessorException)
        {
            throw new AssetPipelineException($"Importer '{importer.GetType().Name}' threw processor exception - use {nameof(AssetImporterException)} instead");
        }
        catch (AssetPipelineException)
        {
            throw;
        }
        catch (EngineException)
        {
            throw new AssetPipelineException($"Importer '{importer.GetType().Name}' threw {nameof(EngineException)} - use {nameof(AssetImporterException)} for import errors");
        }
        catch (Exception)
        {
            throw;
        }
    }

    private object ProcessAssetInternal(IAssetProcessor processor, object rawAsset, string path)
    {
        try
        {
            _logger.Trace($"Processing {rawAsset.GetType().Name} with {processor.GetType().Name}");

            var asset = processor.Process(rawAsset);
            if (asset is null)
            {
                throw new AssetPipelineException($"Processor '{processor.GetType().Name}' returned null for '{path}' - processing failed");
            }

            _logger.Trace($"Processing successful: {rawAsset.GetType().Name} -> {asset.GetType().Name}");
            return asset;
        }
        catch (AssetProcessorException)
        {
            throw;
        }
        catch (AssetImporterException)
        {
            throw new AssetPipelineException($"Processor '{processor.GetType().Name}' threw importer exception - use {nameof(AssetProcessorException)} instead");
        }
        catch (EngineException)
        {
            throw new AssetPipelineException($"Processor '{processor.GetType().Name}' threw {nameof(EngineException)} - use {nameof(AssetProcessorException)} for processing errors");
        }
        catch (Exception ex)
        {
            throw new AssetPipelineException($"Unexpected error in processor '{processor.GetType().Name}' for '{path}' - verify processor logic and input validity", ex);
        }
    }

    private bool TryGetCachedAsset(string path, [NotNullWhen(true)] out object? cachedAsset)
    {
        if (_cachedAssets.TryGetValue(path, out cachedAsset))
        {
            _logger.Trace($"Cache hit: {cachedAsset.GetType().Name}");
            return true;
        }

        return false;
    }
}