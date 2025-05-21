using System.Diagnostics.CodeAnalysis;
using PokeSharp.Core.Exceptions;
using PokeSharp.Core.Services;

namespace PokeSharp.Assets;

public sealed class AssetPipeline
{
    private readonly IAssetImporter[] _importers;
    private readonly Dictionary<(Type inputType, Type outputType), IAssetProcessor> _processors;

    private readonly ReflectionManager _reflectionManager;
    private readonly Dictionary<string, object> _cachedAssets;

    public AssetPipeline(ReflectionManager reflectionManager)
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

    private Dictionary<(Type inputType, Type outputType), IAssetProcessor> LoadProcessors()
    {
        IAssetProcessor[] processors = _reflectionManager.InstantiateClassesOfType<IAssetProcessor>();
        return processors.ToDictionary(x => (x.InputType, x.OutputType));
    }

    public T Load<T>(string path) where T : class
    {
        if (TryGetCachedAsset(path, out T? cachedAsset))
        {
            return cachedAsset;
        }

        string ext = Path.GetExtension(path);

        IAssetImporter? importer = _importers.FirstOrDefault(x => x.CanImport(ext));
        if (importer == null)
        {
            throw new EngineException($"""
                No asset importer found for file extension '{ext}'.
                The format may not be supported, or the extension might be incorrect. Please verify the file type.
            """);
        }

        object? rawAsset;
        try
        {
            rawAsset = importer.Import(path);
        }
        catch (Exception ex)
        {
            throw new EngineException($"""
                An exception occurred while importing asset at '{path}' using '{importer.GetType().Name}'.
                Please ensure the file format is valid and that the importer supports it.
            """, ex);
        }

        if (rawAsset == null)
        {
            throw new EngineException($"""
                Failed to import asset at '{path}' using importer '{importer.GetType().Name}'.
                The importer returned null, which indicates an unexpected error during import.
            """);
        }

        Type rawAssetType = rawAsset.GetType();
        if (!_processors.TryGetValue((rawAssetType, typeof(T)), out IAssetProcessor? processor))
        {
            throw new EngineException($"""
                No processor available to convert from '{rawAssetType.Name}' to '{typeof(T).Name}'.
                Note: the asset pipeline does not support multi-step conversions (e.g., A → B → C).
                A direct processor must exist between the raw and final asset type.
            """);
        }

        object? asset;
        try
        {
            asset = processor.Process(rawAsset);
        }
        catch (Exception ex)
        {
            throw new EngineException($"""
                An exception occurred while processing asset at '{path}' using '{processor.GetType().Name}'.
                Check the processor logic and input data format.
            """, ex);
        }

        if (asset == null)
        {
            throw new EngineException($"""
                Failed to process asset at '{path}' using processor '{processor.GetType().Name}'.
                The processor returned null, indicating an internal failure during processing.
            """);
        }

        if (asset is not T assetT)
        {
            throw new EngineException($"""
                The processed asset is of type '{asset.GetType().Name}', which is not compatible with the expected type '{typeof(T).Name}'.
                Ensure the correct processor is used and that the expected output type is accurate.
            """);
        }

        return assetT;
    }

    private bool TryGetCachedAsset<T>(string path, [NotNullWhen(true)] out T? cachedAsset) where T : class
    {
        cachedAsset = null;

        if (!_cachedAssets.TryGetValue(path, out object? asset))
        {
            return false;
        }

        if (asset is not T assetT)
        {
            throw new EngineException($"""
                Cached asset at '{path}' is of type '{asset.GetType().Name}', which does not match the expected type '{typeof(T).Name}'.
                Make sure to request the asset with the correct type.
            """);
        }

        cachedAsset = assetT;
        return true;
    }
}