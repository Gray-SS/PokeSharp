using Microsoft.Xna.Framework;
using PokeSharp.Assets.VFS;
using PokeSharp.Assets.VFS.Events;
using PokeSharp.Assets.VFS.Services;
using PokeSharp.Assets.VFS.Volumes;
using PokeSharp.Core.Logging;
using PokeSharp.Editor.ContentBrowser.Events;
using PokeSharp.Engine.Core;

namespace PokeSharp.Editor.ContentBrowser.Services;

public sealed class ContentCacheService : IContentCacheService, IEngineHook
{
    public event EventHandler<ContentCacheRefreshedArgs>? OnCacheRefreshed;

    public IReadOnlyCollection<IVirtualFile> Files => _cachedFiles;
    public IReadOnlyCollection<IVirtualVolume> Volumes => _cachedVolumes;
    public IReadOnlyCollection<IVirtualDirectory> Directories => _cachedDirectories;

    private ContentScope _invalidationScope;
    private readonly Queue<VirtualPath> _invalidatedPaths;

    private readonly List<IVirtualFile> _cachedFiles;
    private readonly List<IVirtualVolume> _cachedVolumes;
    private readonly List<IVirtualDirectory> _cachedDirectories;

    private readonly Logger _logger;
    private readonly IContentNavigator _navigator;
    private readonly IVirtualVolumeManager _volumesManager;

    public ContentCacheService(
        Logger logger,
        IContentNavigator navigator,
        IVirtualVolumeManager volumesManager)
    {
        _invalidationScope = ContentScope.None;
        _invalidatedPaths = new Queue<VirtualPath>();
        _cachedFiles = new List<IVirtualFile>();
        _cachedVolumes = new List<IVirtualVolume>();
        _cachedDirectories = new List<IVirtualDirectory>();
        _logger = logger;

        _navigator = navigator;
        _navigator.CurrentPathChanged += OnCurrentPathChanged;

        _volumesManager = volumesManager;
        _volumesManager.OnVolumeMounted += OnVolumeMounted;
        _volumesManager.OnVolumeUnmounted += OnVolumeUnmounted;
        _volumesManager.OnFileSystemChanged += OnFileSystemChanged;
    }

    private void OnFileSystemChanged(object? sender, FileSystemChangedArgs e)
    {
        VirtualPath entryChangedPath = e.VirtualPath;
        _logger.Trace($"{(e.VirtualPath.IsFile ? "File" : "Directory")} {e.ChangeType.ToString().ToLower()} '{entryChangedPath}'");

        // TODO: Need a clean way of getting the state of the editor.
        if (_navigator.CurrentPath == null)
        {
            _logger.Warn("The navigator current path is null.");
            return;
        }

        VirtualPath currentPath = _navigator.CurrentPath;
        bool isChild = entryChangedPath.IsDirectory && entryChangedPath.IsParentOf(currentPath);
        if (!currentPath.IsDirectParentOf(entryChangedPath) && !isChild)
        {
            _logger.Trace("Changed entry is outside current path scope. Ignoring.");
            return;
        }

        _logger.Trace("File change in current directory scope. Invalidating current directory.");
        Invalidate(ContentScope.CurrentDirectory);
    }

    private void OnVolumeMounted(object? sender, VirtualVolumeEventArgs e)
    {
        _logger.Trace("Volume mounted. Invalidating volumes.");
        Invalidate(ContentScope.Volumes);
    }

    private void OnVolumeUnmounted(object? sender, VirtualVolumeEventArgs e)
    {
        _logger.Trace("Volume unmounted. Invalidating volumes.");
        Invalidate(ContentScope.Volumes);
    }

    private void OnCurrentPathChanged(object? sender, VirtualPath e)
    {
        _logger.Trace("Navigator path changed. Invalidating current directory.");
        Invalidate(ContentScope.CurrentDirectory);
    }

    public void Invalidate(ContentScope scope)
    {
        if (_invalidationScope.HasFlag(scope))
        {
            _logger.Trace($"Scope {scope} already invalidated. Ignoring.");
            return;
        }

        _invalidationScope |= scope;
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

        OnCacheRefreshed?.Invoke(this, new ContentCacheRefreshedArgs(scope));
    }

    private void RefreshCurrentDirectory()
    {
        _logger.Trace("Refreshing current directory");

        _cachedFiles.Clear();
        _cachedDirectories.Clear();

        IVirtualDirectory directory = _navigator.GetCurrentDirectory();
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
        {
            _cachedVolumes.Add(volume);
        }

        _logger.Trace("Volumes successfully refresh");
    }

    public void Initialize()
    {
    }

    public void Update(GameTime gameTime)
    {
        if (_invalidationScope != ContentScope.None)
        {
            Refresh(_invalidationScope);
            _invalidationScope = ContentScope.None;
        }
    }

    public void Draw(GameTime gameTime)
    {
    }
}