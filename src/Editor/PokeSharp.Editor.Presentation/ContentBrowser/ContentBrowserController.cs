using PokeSharp.Assets.VFS;
using PokeSharp.Assets.VFS.Events;
using PokeSharp.Assets.VFS.Services;
using PokeSharp.Core.Logging;
using PokeSharp.Editor.Application.ContentBrowser;

namespace PokeSharp.Editor.Presentation.ContentBrowser;

public sealed class ContentBrowserController : IDisposable
{
    private bool _isDisposed;

    private readonly Logger _logger;
    private readonly ITickSource _tickSource;
    private readonly IVirtualVolumeManager _volumeManager;

    private readonly IContentBrowserView _view;
    private readonly IContentBrowserCache _cache;
    private readonly IContentBrowserNavigator _navigator;

    public ContentBrowserController(
        Logger logger,
        ITickSource tickSource,
        IVirtualVolumeManager volumeManager,
        IContentBrowserView view,
        IContentBrowserCache cache,
        IContentBrowserNavigator navigator
    )
    {
        _logger = logger;
        _tickSource = tickSource;
        _volumeManager = volumeManager;

        _view = view;
        _cache = cache;
        _navigator = navigator;

        _tickSource.OnTick += OnTick;
        _cache.OnRefresh += OnCacheRefreshed;
        _navigator.OnPathChanged += OnPathChanged;

        _volumeManager.OnVolumeMounted += OnVolumeMounted;
        _volumeManager.OnVolumeUnmounted += OnVolumeUnmounted;
        _volumeManager.OnFileSystemChanged += OnFileSystemChanged;
    }

    private void OnCacheRefreshed(object? sender, ContentBrowserCacheRefreshed e)
    {
        var models = new List<ContentViewModel>();
        foreach (IVirtualDirectory directory in _cache.Directories)
        {
            var model = new ContentViewModel(directory.Name);
            models.Add(model);
        }

        _view.SetItems(models);
    }

    private void OnPathChanged(object? sender, ContentPathChangedArgs e)
    {
        _cache.Invalidate(ContentScope.CurrentDirectory);
    }

    private void OnVolumeMounted(object? sender, VirtualVolumeEventArgs e)
    {
        _cache.Invalidate(ContentScope.Volumes);
    }

    private void OnVolumeUnmounted(object? sender, VirtualVolumeEventArgs e)
    {
        _cache.Invalidate(ContentScope.Volumes);
    }

    private void OnFileSystemChanged(object? sender, FileSystemChangedArgs e)
    {
        VirtualPath newPath = e.VirtualPath;
        _logger.Trace($"File system changed received. Processing ({e.ChangeType} {newPath})");

        if (_navigator.CurrentPath.IsDirectParentOf(newPath))
            _cache.Invalidate(ContentScope.CurrentDirectory);
    }

    private void OnTick()
    {
        if (_cache.HaveInvalidatedScopes)
        {
            _cache.RefreshInvalidatedScopes();
        }
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _tickSource.OnTick -= OnTick;
            _cache.OnRefresh -= OnCacheRefreshed;
            _navigator.OnPathChanged -= OnPathChanged;
            _volumeManager.OnVolumeMounted -= OnVolumeMounted;
            _volumeManager.OnVolumeUnmounted -= OnVolumeUnmounted;
            _volumeManager.OnFileSystemChanged -= OnFileSystemChanged;

            _isDisposed = true;
        }
    }
}