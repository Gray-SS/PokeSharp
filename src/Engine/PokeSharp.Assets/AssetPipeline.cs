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

    public event EventHandler? AssetImported;

    private readonly IAssetImporter[] _importers;
    private readonly IAssetProcessor[] _processors;
    private readonly IAssetWriter[] _writers;

    private readonly Logger _logger;
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
        Logger logger)
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
        _logger.Debug("Reimporting assets");

        _metadataStore.EnterBulkMode();
        _metadataStore.DeleteAll();

        var volumes = _volumeManager.GetVolumes().ToArray();
        foreach (IVirtualVolume volume in volumes)
        {
            if (volume.Id == "library")
                continue;

            IVirtualDirectory directory = _vfs.GetDirectory(volume.RootPath);
            ImportRecursive(directory);
        }

        _metadataStore.ExitBulkMode();

        _logger.Debug("Assets successfully reimported.");
    }

    private void ImportRecursive(IVirtualDirectory directory)
    {
        foreach (IVirtualDirectory child in directory.GetDirectories())
        {
            ImportRecursive(child);
        }

        foreach (IVirtualFile child in directory.GetFiles())
        {
            Import(child.Path);
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

    public void Import(VirtualPath assetPath)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(assetPath);

            _logger.Debug($"Importing asset at path '{assetPath}'");

            IVirtualFile file = _vfs.GetFile(assetPath);
            if (!file.Exists)
                throw new AssetPipelineException($"File not found: '{assetPath}'");

            AssetMetadata metadata;
            if (!_metadataStore.Exists(assetPath))
            {
                _logger.Debug("No asset metadata found. Creating new asset metadata");

                Guid assetId = Guid.NewGuid();
                metadata = new AssetMetadata
                {
                    Id = assetId,
                    ResourcePath = assetPath
                };

                _logger.Trace($"Asset metadata instantiated ({assetId})");

                if (string.IsNullOrEmpty(assetPath.Extension))
                {
                    _logger.Warn("No importer can be assigned to an extension-less file.");
                    metadata.ErrorMessage = "No importer can be assigned to an extension-less file.";

                    _metadataStore.Save(assetPath, metadata);
                    return;
                }

                string extension = assetPath.Extension;
                IAssetImporter? foundImporter = _importers.FirstOrDefault(x => x.SupportedExtensions.Split(',').Any(ext => string.Equals(extension, ext, StringComparison.OrdinalIgnoreCase)));
                if (foundImporter == null)
                {
                    _logger.Warn($"No importer found for file extension '{extension}'");
                    metadata.ErrorMessage = $"No importer found for file extension '{extension}'";

                    _metadataStore.Save(assetPath, metadata);
                    return;
                }
                _logger.Trace($"Importer supporting {extension} found: '{foundImporter.GetType().Name}'");

                IAssetProcessor? foundProcessor = _processors.FirstOrDefault(x => foundImporter.ProcessorType == x.GetType());
                if (foundProcessor == null)
                {
                    _logger.Warn($"No processor paired with importer '{foundImporter.GetType().Name}' was found");
                    metadata.ErrorMessage = $"No processor paired with importer '{foundImporter.GetType().Name}' was found";

                    _metadataStore.Save(assetPath, metadata);
                    return;
                }
                _logger.Trace($"Processor paired to importer found and resolved: '{foundProcessor.GetType().Name}'");

                metadata.Importer = foundImporter;
                metadata.AssetType = foundProcessor.OutputType;
            }
            else
            {
                _logger.Debug($"Metadata found for asset '{assetPath}'. Loading metadata");
                metadata = _metadataStore.Load(assetPath);
            }

            IAssetImporter importer = metadata.Importer;

            object? rawAsset;
            try
            {
                _logger.Trace($"Importing asset with importer: '{importer.GetType().Name}'");
                rawAsset = importer.Import(file);
                if (rawAsset == null)
                {
                    _logger.Warn($"Importer '{importer.GetType().Name}' returned null for '{assetPath}' - file may be corrupted or unsupported");
                    metadata.ErrorMessage = "Something went wrong in the import process. Check the logs for more details.";

                    _logger.Debug("Saving metadata as a fallback.");
                    _metadataStore.Save(assetPath, metadata);
                    return;
                }
            }
            catch (AssetImporterException ex)
            {
                _logger.Warn($"Import failed for '{assetPath}'. {ex.Message}");
                metadata.ErrorMessage = $"Import failed: {ex.Message}";

                _logger.Debug("Saving metadata as a fallback.");
                _metadataStore.Save(assetPath, metadata);
                return;
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error occured while importing: {ex.GetType().Name}: {ex.Message}");
                _logger.Debug($"{ex.StackTrace ?? "No stack trace available"}");
                metadata.ErrorMessage = $"Unexpected error occured while importing: {ex.GetType().Name}: {ex.Message}";

                _logger.Debug("Saving metadata as a fallback.");
                _metadataStore.Save(assetPath, metadata);
                return;
            }

            Type rawAssetType = rawAsset.GetType();
            _metadataStore.Save(assetPath, metadata);
            AssetImported?.Invoke(this, EventArgs.Empty);
            _logger.Debug($"Asset of type '{rawAssetType.Name}' imported.");
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error($"Asset import failed - Parameter '{ex.ParamName}' is null");
        }
        catch (Exception ex)
        {
            _logger.Error($"Unexpected error while importing '{assetPath}': {ex.GetType().Name} - {ex.Message}");
            _logger.Debug($"{ex.StackTrace ?? "No stack trace available"}");
        }
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
}