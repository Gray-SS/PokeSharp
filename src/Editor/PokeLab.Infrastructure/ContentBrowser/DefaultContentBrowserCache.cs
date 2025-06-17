using PokeCore.Logging;
using PokeCore.IO;
using PokeCore.IO.Services;
using PokeCore.IO.Volumes;
using PokeLab.Application.ContentBrowser;

namespace PokeLab.Infrastructure.ContentBrowser;

public sealed class DefaultContentBrowserCache(
    Logger<DefaultContentBrowserCache> logger,
    IVirtualVolumeManager volumeManager,
    IContentBrowserNavigator navigator
) : IContentBrowserCache
{
    public IReadOnlyCollection<IVirtualFile> Files => _cachedFiles;
    public IReadOnlyCollection<IVirtualVolume> Volumes => _cachedVolumes;
    public IReadOnlyCollection<IVirtualDirectory> Directories => _cachedDirectories;

    public bool HaveInvalidatedScopes => _invalidationScope != ContentScope.None;

    private ContentScope _invalidationScope;

    private readonly List<IVirtualFile> _cachedFiles = new();
    private readonly List<IVirtualVolume> _cachedVolumes = new();
    private readonly List<IVirtualDirectory> _cachedDirectories = new();

    public void Clear()
    {
        _cachedFiles.Clear();
        _cachedVolumes.Clear();
        _cachedDirectories.Clear();
    }

    public void Invalidate(ContentScope scope)
    {
        if (_invalidationScope.HasFlag(scope))
        {
            logger.Trace($"Content scope '{scope}' already invalidated. Ignoring.");
            return;
        }

        _invalidationScope |= scope;
        logger.Trace($"Content scope '{scope}' invalidated.");
    }

    public void Refresh(ContentScope scope)
    {
        if (scope == ContentScope.None)
            return;

        logger.Trace("Refreshing content cache");

        if (scope.HasFlag(ContentScope.Volumes))
            RefreshVolumes();

        if (scope.HasFlag(ContentScope.CurrentDirectory))
            RefreshCurrentDirectory();
    }

    public void RefreshInvalidatedScopes()
    {
        Refresh(_invalidationScope);
        _invalidationScope = ContentScope.None;
    }

    private void RefreshCurrentDirectory()
    {
        logger.Trace("Refreshing current directory");

        _cachedFiles.Clear();
        _cachedDirectories.Clear();

        IVirtualDirectory directory = navigator.CurrentDirectory;
        if (directory.Exists)
        {
            _cachedFiles.AddRange(directory.GetFiles());
            _cachedDirectories.AddRange(directory.GetDirectories());
            logger.Trace($"Current directory successfully refreshed");
        }
        else logger.Warn($"Current directory does not exists at path: '{directory.Path}'");
    }

    private void RefreshVolumes()
    {
        logger.Trace("Refreshing volumes");

        _cachedVolumes.Clear();
        foreach (IVirtualVolume volume in volumeManager.GetVolumes())
            _cachedVolumes.Add(volume);

        logger.Trace("Volumes successfully refresh");
    }
}