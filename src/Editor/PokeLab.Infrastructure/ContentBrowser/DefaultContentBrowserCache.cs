using PokeEngine.Assets.VFS;
using PokeEngine.Assets.VFS.Services;
using PokeEngine.Assets.VFS.Volumes;
using PokeCore.Hosting.Logging;
using PokeLab.Application.ContentBrowser;

namespace PokeLab.Infrastructure.ContentBrowser;

public sealed class DefaultContentBrowserCache : IContentBrowserCache
{
    public IReadOnlyCollection<IVirtualFile> Files => _cachedFiles;
    public IReadOnlyCollection<IVirtualVolume> Volumes => _cachedVolumes;
    public IReadOnlyCollection<IVirtualDirectory> Directories => _cachedDirectories;

    public bool HaveInvalidatedScopes => _invalidationScope != ContentScope.None;

    public event EventHandler<ContentBrowserCacheRefreshed>? OnRefresh;

    private ContentScope _invalidationScope;

    private readonly Logger _logger;
    private readonly IVirtualVolumeManager _volumesManager;
    private readonly IContentBrowserNavigator _navigator;

    private readonly List<IVirtualFile> _cachedFiles;
    private readonly List<IVirtualVolume> _cachedVolumes;
    private readonly List<IVirtualDirectory> _cachedDirectories;

    public DefaultContentBrowserCache(
        Logger logger,
        IVirtualVolumeManager volumeManager,
        IContentBrowserNavigator navigator)
    {
        _logger = logger;
        _navigator = navigator;
        _volumesManager = volumeManager;

        _cachedFiles = new List<IVirtualFile>();
        _cachedVolumes = new List<IVirtualVolume>();
        _cachedDirectories = new List<IVirtualDirectory>();
    }

    public void Invalidate(ContentScope scope)
    {
        if (_invalidationScope.HasFlag(scope))
        {
            _logger.Trace($"Content scope '{scope}' already invalidated. Ignoring.");
            return;
        }

        _invalidationScope |= scope;
        _logger.Trace($"Content scope '{scope}' invalidated.");
    }

    public void Refresh(ContentScope scope)
    {
        if (scope == ContentScope.None)
            return;

        _logger.Trace("Refreshing content cache");

        if (scope.HasFlag(ContentScope.Volumes))
            RefreshVolumes();

        if (scope.HasFlag(ContentScope.CurrentDirectory))
            RefreshCurrentDirectory();

        OnRefresh?.Invoke(this, new ContentBrowserCacheRefreshed(scope));
    }

    public void RefreshInvalidatedScopes()
    {
        Refresh(_invalidationScope);
        _invalidationScope = ContentScope.None;
    }

    private void RefreshCurrentDirectory()
    {
        _logger.Trace("Refreshing current directory");

        _cachedFiles.Clear();
        _cachedDirectories.Clear();

        IVirtualDirectory directory = _navigator.CurrentDirectory;
        if (directory.Exists)
        {
            _cachedFiles.AddRange(directory.GetFiles());
            _cachedDirectories.AddRange(directory.GetDirectories());
            _logger.Trace("Current directory successfully refreshed");
        }
        else _logger.Warn($"Current directory does not exists at path: '{directory.Path}'");
    }

    private void RefreshVolumes()
    {
        _logger.Trace("Refreshing volumes");

        _cachedVolumes.Clear();
        foreach (IVirtualVolume volume in _volumesManager.GetVolumes())
            _cachedVolumes.Add(volume);

        _logger.Trace("Volumes successfully refresh");
    }
}