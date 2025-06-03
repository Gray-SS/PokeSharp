using System.Diagnostics.CodeAnalysis;
using PokeSharp.Assets.Exceptions;
using PokeSharp.Assets.Services;
using PokeSharp.Assets.VFS;
using PokeSharp.Assets.VFS.Services;
using PokeSharp.Assets.VFS.Volumes;
using PokeSharp.Core.Exceptions;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Services;

namespace PokeSharp.Assets;

public sealed class AssetPipeline
{
    public IReadOnlyCollection<object> LoadedAssets => _cachedAssets.Values;

    private readonly IAssetImporter[] _importers;
    private readonly IAssetProcessor[] _processors;
    private readonly IAssetWriter[] _writers;

    private readonly ILogger _logger;
    private readonly IVirtualFileSystem _vfs;
    private readonly IVirtualVolumeManager _volumeManager;
    private readonly IAssetMetadataStore _metadataStore;
    private readonly IReflectionManager _reflectionManager;
    private readonly Dictionary<string, object> _cachedAssets;

    public AssetPipeline(
        IReflectionManager reflectionManager,
        IAssetMetadataStore metadataStore,
        IVirtualVolumeManager volumeManager,
        IVirtualFileSystem vfs,
        ILogger logger)
    {
        _vfs = vfs;
        _volumeManager = volumeManager;

        _logger = logger;
        _metadataStore = metadataStore;
        _reflectionManager = reflectionManager;

        _importers = LoadImporters();
        _processors = LoadProcessors();
        _writers = LoadWriters();

        _cachedAssets = new Dictionary<string, object>();
    }

    public void ReimportAll()
    {
        _metadataStore.DeleteAll();

        var volumes = _volumeManager.GetVolumes().ToArray();
        foreach (IVirtualVolume volume in volumes)
        {
            if (volume.Id == "library")
                continue;

            IVirtualDirectory directory = _vfs.GetDirectory(volume.RootPath);
            ImportRecursive(directory);
        }
    }

    private void ImportRecursive(IVirtualDirectory directory)
    {
        foreach (IVirtualDirectory child in directory.GetDirectories())
        {
            ImportRecursive(child);
        }

        foreach (IVirtualFile child in directory.GetFiles())
        {
            TryImport(child.Path);
        }
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

    public bool TryImport(VirtualPath assetPath)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(assetPath);

            _logger.Debug($"Importing asset: '{assetPath}'");

            IVirtualFile file = _vfs.GetFile(assetPath);
            if (!file.Exists)
                throw new AssetPipelineException($"File not found: '{assetPath}'");

            string ext = assetPath.Extension;
            if (string.IsNullOrEmpty(ext))
                throw new AssetPipelineException($"File extension required for asset: '{assetPath}'");

            AssetMetadata metadata = GetOrCreateMetadata(assetPath);
            object imported = ImportAssetInternal(metadata.Importer!, file);
            object processed = ProcessAssetInternal(metadata.Processor!, imported, assetPath);

            // ReimportAll();

            _logger.Debug($"Asset imported: {assetPath}, {_metadataStore.GetMetadataPath(assetPath)}");
            return true;
        }
        catch (AssetProcessorException ex)
        {
            _logger.Error($"Asset processing failed for '{assetPath}': {ex.Message}");
        }
        catch (AssetImporterException ex)
        {
            _logger.Error($"Asset import failed for '{assetPath}': {ex.Message}");
        }
        catch (AssetPipelineException ex)
        {
            _logger.Error($"Asset loading failed for '{assetPath}': {ex.Message}");
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error($"Asset import failed - The parameter '{ex.ParamName}' was null");
        }
        catch (Exception ex)
        {
            _logger.Error($"Unexpected error while importing '{assetPath}': {ex.GetType().Name} - {ex.Message}");
            _logger.Debug($"{ex.StackTrace ?? "No stack trace available"}");
        }

        return false;
    }

    public bool TryMove(VirtualPath srcPath, VirtualPath destPath)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(srcPath);

            _logger.Debug($"Trying to move '{srcPath}' to '{destPath}'");

            IVirtualEntry entry = _vfs.GetEntry(srcPath);
            if (!entry.Exists)
                throw new AssetPipelineException($"Entry not found: '{srcPath}'");

            entry.Move(destPath);
            ReimportAll();

            _logger.Debug($"Moved '{srcPath}' to '{destPath}' successfully");
            return true;
        }
        catch (AssetPipelineException ex)
        {
            _logger.Error($"Moving '{srcPath}' to '{destPath}' failed: {ex.Message}");
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error($"Moving '{srcPath}' to '{destPath}' failed: The parameter '{ex.ParamName}' was null");
        }
        catch (Exception ex)
        {
            _logger.Error($"Unexpected error while moving '{srcPath}' to '{destPath}': {ex.GetType().Name} - {ex.Message}");
            _logger.Debug($"{ex.StackTrace ?? "No stack trace available"}");
        }

        return false;
    }

    public bool TryDelete(VirtualPath srcPath)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(srcPath);

            _logger.Debug($"Trying to delete '{srcPath}'");

            IVirtualEntry entry = _vfs.GetEntry(srcPath);
            if (!entry.Exists)
                throw new AssetPipelineException($"Entry not found: '{srcPath}'");

            entry.Delete();
            ReimportAll();

            _logger.Debug($"Deleted '{srcPath}' successfully");
            return true;
        }
        catch (AssetPipelineException ex)
        {
            _logger.Error($"Moving '{srcPath}' failed: {ex.Message}");
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error($"Moving '{srcPath}' failed: The parameter '{ex.ParamName}' was null");
        }
        catch (Exception ex)
        {
            _logger.Error($"Unexpected error while deleting '{srcPath}': {ex.GetType().Name} - {ex.Message}");
            _logger.Debug($"{ex.StackTrace ?? "No stack trace available"}");
        }

        return false;
    }

    public bool TryDuplicate(VirtualPath srcPath)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(srcPath);

            _logger.Debug($"Trying to duplicate '{srcPath}'");

            IVirtualEntry entry = _vfs.GetEntry(srcPath);
            if (!entry.Exists)
                throw new AssetPipelineException($"Entry not found: '{srcPath}'");

            entry.Duplicate();
            ReimportAll();

            _logger.Debug($"Duplicated '{srcPath}' successfully");
            return true;
        }
        catch (AssetPipelineException ex)
        {
            _logger.Error($"Duplicated '{srcPath}' failed: {ex.Message}");
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error($"Duplicated '{srcPath}' failed: The parameter '{ex.ParamName}' was null");
        }
        catch (Exception ex)
        {
            _logger.Error($"Unexpected error while duplicating '{srcPath}': {ex.GetType().Name} - {ex.Message}");
            _logger.Debug($"{ex.StackTrace ?? "No stack trace available"}");
        }

        return false;
    }

    public bool TryRename(VirtualPath srcPath, string name)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(srcPath);

            _logger.Debug($"Trying to rename '{srcPath}' to '{name}'");

            IVirtualEntry entry = _vfs.GetEntry(srcPath);
            if (!entry.Exists)
                throw new AssetPipelineException($"Entry not found: '{srcPath}'");

            entry.Rename(name);
            ReimportAll();

            _logger.Debug($"Renaming '{srcPath}' successfully");
            return true;
        }
        catch (AssetPipelineException ex)
        {
            _logger.Error($"Renaming '{srcPath}' failed: {ex.Message}");
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error($"Renaming '{srcPath}' failed: The parameter '{ex.ParamName}' was null");
        }
        catch (Exception ex)
        {
            _logger.Error($"Unexpected error while renaming '{srcPath}': {ex.GetType().Name} - {ex.Message}");
            _logger.Debug($"{ex.StackTrace ?? "No stack trace available"}");
        }

        return false;
    }

    public bool HasMetadata(VirtualPath vpath)
    {
        try
        {
            return _metadataStore.Exists(vpath);
        }
        catch (Exception ex)
        {
            _logger.Error($"Unexpected error while checking if asset at path '{vpath}' has metadata: {ex.GetType().Name}: {ex.Message}");
            _logger.Debug($"{ex.StackTrace ?? "No stack trace available"}");
        }

        return false;
    }

    public AssetMetadata CreateMetadata(VirtualPath assetPath)
    {
        Guid id = Guid.NewGuid();

        // TODO: Find a way to get the asset type
        var metadata = new AssetMetadata(id);
        metadata.Importer = GetAssetImporter(assetPath);
        metadata.Processor = GetDefaultProcessorFromImporter(metadata.Importer); // Is processor really needed ?
        metadata.ResourcePath = assetPath;
        metadata.AssetType = metadata.Processor.OutputType;

        _logger.Trace($"Metadata created for asset of type '{metadata.AssetType.Name}'");

        _metadataStore.Save(assetPath, metadata);
        return metadata;
    }

    public AssetMetadata GetMetadata(VirtualPath assetPath)
    {
        return _metadataStore.Load(assetPath);
    }

    public AssetMetadata GetOrCreateMetadata(VirtualPath assetPath)
    {
        if (_metadataStore.Exists(assetPath))
            return _metadataStore.Load(assetPath);

        return CreateMetadata(assetPath);
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

    private IAssetImporter GetAssetImporter(VirtualPath path)
    {
        _logger.Trace($"Finding importer for extension: '{path.Extension}'");

        var importer = _importers.FirstOrDefault(x => x.CanImport(path));
        if (importer is null)
        {
            throw new AssetPipelineException($"No importer found for extension '{path.Extension}' - asset type may not be supported");
        }

        _logger.Trace($"Using importer: {importer.GetType().Name}");
        return importer;
    }

    private IAssetProcessor GetDefaultProcessorFromImporter(IAssetImporter importer)
    {
        Type? processorType;
        try
        {
            processorType = importer.ProcessorType;
        }
        catch (NotImplementedException)
        {
            throw new AssetImporterException(
                $"The default processor for importer '{importer.GetType().Name}' was not implemented - ensure you're defining a valid processor inside your importer");
        }

        if (processorType is null)
        {
            throw new AssetPipelineException(
                $"The default processor for importer '{importer.GetType().Name}' was set to null - ensure you're using a valid processor type");
        }

        if (!processorType.IsAssignableTo(typeof(IAssetProcessor)))
        {
            throw new AssetPipelineException(
                $"The default processor for importer '{importer.GetType().Name}' isn't assignable to {nameof(IAssetProcessor)} - ensure you're using a valid processor type");
        }

        _logger.Trace($"Finding processor '{processorType.Name}' for importer '{importer.GetType().Name}'");

        var processor = _processors.FirstOrDefault(x => x.GetType() == processorType);
        if (processor is null)
        {
            throw new AssetPipelineException(
                $"Processor '{processorType.Name}' not found for importer '{importer.GetType().Name}' - Available processors are:\n {string.Join("-", _processors.Select(x => x.GetType().Name))}");
        }

        _logger.Trace($"Using processor: {processor.GetType().Name} ({processor.InputType.Name} -> {processor.OutputType.Name})");
        return processor;
    }

    private object ImportAssetInternal(IAssetImporter importer, IVirtualFile file)
    {
        try
        {
            _logger.Trace($"Importing with {importer.GetType().Name}");

            var rawAsset = importer.Import(file);
            if (rawAsset is null)
            {
                throw new AssetPipelineException($"Importer '{importer.GetType().Name}' returned null for '{file}' - file may be corrupted or unsupported");
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

    private object ProcessAssetInternal(IAssetProcessor processor, object rawAsset, VirtualPath path)
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
        catch (Exception)
        {
            throw;
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