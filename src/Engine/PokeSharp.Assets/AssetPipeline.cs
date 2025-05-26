using System.Diagnostics.CodeAnalysis;
using PokeSharp.Assets.Exceptions;
using PokeSharp.Assets.VFS;
using PokeSharp.Core.Exceptions;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Services;

namespace PokeSharp.Assets;

public sealed class AssetPipeline
{
    public string BasePath { get; set; }

    public IReadOnlyCollection<object> LoadedAssets => _cachedAssets.Values;

    private readonly IAssetImporter[] _importers;
    private readonly IAssetProcessor[] _processors;
    private readonly IAssetWriter[] _writers;

    private readonly ILogger _logger;
    private readonly IVirtualFileSystem _vfs;
    private readonly IReflectionManager _reflectionManager;
    private readonly Dictionary<string, object> _cachedAssets;

    public AssetPipeline(IReflectionManager reflectionManager, IVirtualFileSystem vfs, ILogger logger)
    {
        BasePath = AppContext.BaseDirectory;

        _vfs = vfs;
        _logger = logger;
        _reflectionManager = reflectionManager;

        _importers = LoadImporters();
        _processors = LoadProcessors();
        _writers = LoadWriters();

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

    private IAssetWriter[] LoadWriters()
    {
        return _reflectionManager.InstantiateClassesOfType<IAssetWriter>();
    }

    public object? Load(string path)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new AssetPipelineException("Asset path cannot be null or empty");
            }

            path = Path.Combine(BasePath, path);
            _logger.Debug($"Loading asset: '{path}'");

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
            _logger.Error($"Asset loading failed for '{path}': {ex.Message}");
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

    public bool Save(object asset, string savePath)
    {
        try
        {
            if (asset == null)
                throw new AssetPipelineException("Asset cannot be null");

            if (string.IsNullOrWhiteSpace(savePath))
                throw new AssetPipelineException("Save path cannot be null or empty");

            savePath = Path.Combine(BasePath, savePath);
            _logger.Info($"Saving asset of type '{asset.GetType().Name}' to '{savePath}'");

            string fullPath = Path.GetFullPath(savePath);
            _logger.Trace($"Resolved full path: '{fullPath}'");

            if (File.Exists(fullPath))
                throw new AssetPipelineException($"File already existing at path: '{fullPath}' - skipping to avoid file overriding");

            IAssetWriter writer = GetAssetWriterFromAsset(asset);
            writer.Write(asset, fullPath);

            _logger.Info($"Asset written successfully: {asset.GetType().Name} -> '{savePath}'");
            return true;
        }
        catch (AssetPipelineException ex)
        {
            _logger.Error($"Asset pipeline error saving '{asset.GetType().Name}' to '{savePath}': {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Unexpected error saving '{asset.GetType().Name}' to '{savePath}': {ex.Message}");
            _logger.Debug($"Stack trace: {ex.StackTrace ?? "Not available"}");
        }

        return false;
    }

    private IAssetWriter GetAssetWriterFromAsset(object asset)
    {
        Type assetType = asset.GetType();
        _logger.Trace($"Finding writer for asset: '{assetType.Name}'");

        var writer = _writers.FirstOrDefault(x => x.AssetType == assetType);
        if (writer is null)
        {
            throw new AssetPipelineException($"No writer found for asset of type '{assetType.Name}' - asset type may not be supported");
        }

        _logger.Trace($"Using writer: {writer.GetType().Name}");
        return writer;
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
        Type processorType;
        try
        {
            processorType = importer.ProcessorType;
        }
        catch (NotImplementedException)
        {
            throw new AssetImporterException($"The default processor for importer '{importer.GetType().Name}' was not implemented - ensure you're defining a valid processor inside your importer");
        }

        if (importer.ProcessorType is null)
        {
            throw new AssetPipelineException($"The default processor for importer '{importer.GetType().Name}' was set to null - ensure you're using a valid processor type");
        }

        if (!importer.ProcessorType.IsAssignableTo(typeof(IAssetProcessor)))
        {
            throw new AssetPipelineException($"The default processor for importer '{importer.GetType().Name}' isn't assignable to {typeof(IAssetProcessor).Name} - ensure you're using a valid processor type");
        }

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
        catch (NotImplementedException)
        {
            throw new AssetPipelineException($"Importer '{importer.GetType().Name}' is not implemented - make sure to implement your importer before using it ;)");
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
        catch (NotImplementedException)
        {
            throw new AssetPipelineException($"Processor '{processor.GetType().Name}' was not implemented - make sure to implement your processor before using it ;)");
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