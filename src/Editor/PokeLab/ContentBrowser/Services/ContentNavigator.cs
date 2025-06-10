using PokeEngine.Assets.VFS;
using PokeEngine.Assets.VFS.Services;
using PokeCore.Hosting;
using PokeCore.Hosting.Logging;
using PokeLab.Services;

namespace PokeLab.ContentBrowser.Services;

public sealed class ContentNavigator : IContentNavigator, IDisposable
{
    public VirtualPath CurrentPath
    {
        get => _crntPath;
        set => NavigateTo(value);
    }

    public IVirtualDirectory CurrentDirectory { get; private set; }

    private VirtualPath _crntPath;
    private readonly Logger _logger;
    private readonly IVirtualFileSystem _vfs;
    private readonly Stack<VirtualPath> _backwardStack;
    private readonly Stack<VirtualPath> _forwardStack;
    private readonly IProjectManager _projectManager;

    public event EventHandler<VirtualPath>? CurrentPathChanged;

    public ContentNavigator(
        Logger logger,
        IVirtualFileSystem vfs,
        IProjectManager projectManager)
    {
        CurrentDirectory = null!;

        _vfs = vfs;
        _logger = logger;
        _crntPath = null!;
        _forwardStack = new Stack<VirtualPath>();
        _backwardStack = new Stack<VirtualPath>();

        _projectManager = projectManager;
        _projectManager.ProjectOpened += OnProjectOpened;
    }

    private void OnProjectOpened(object? sender, Project e)
    {
        NavigateToInternal(e.AssetsVolume.RootPath);
        _logger.Trace($"Initialized current path to root asset volume path: {CurrentPath}");
    }

    public bool CanNavigateBack()
    {
        return _backwardStack.Count > 0;
    }

    public bool CanNavigateForward()
    {
        return _forwardStack.Count > 0;
    }

    public bool CanNavigateTo(VirtualPath path)
    {
        return CurrentPath != path && _vfs.DirectoryExists(path);
    }

    public bool NavigateBack()
    {
        _logger.Debug("Navigating back");
        if (!CanNavigateBack())
        {
            _logger.Warn("Cannot navigate back.");
            return false;
        }

        VirtualPath tempPath = CurrentPath;
        VirtualPath backwardPath = _backwardStack.Pop();

        ThrowHelper.Assert(backwardPath.IsDirectory, "Backward path must be a directory");
        if (!_vfs.DirectoryExists(backwardPath))
        {
            _logger.Warn($"Cannot navigate back to '{backwardPath}'. The directory doesn't exists.");
            return false;
        }

        if (backwardPath == tempPath)
        {
            _logger.Warn($"Cannot navigate back to the same path: {tempPath}");
            return false;
        }

        _forwardStack.Push(tempPath);
        NavigateToInternal(backwardPath);

        _logger.Debug($"Successfully navigated back to '{backwardPath}'.");
        return true;
    }

    public bool NavigateForward()
    {
        _logger.Debug("Navigating forward");
        if (!CanNavigateForward())
        {
            _logger.Trace("Cannot navigate forward.");
            return false;
        }

        VirtualPath tempPath = CurrentPath;
        VirtualPath forwardPath = _forwardStack.Pop();

        ThrowHelper.Assert(forwardPath.IsDirectory, "Forward path must be a directory");
        if (!_vfs.DirectoryExists(forwardPath))
        {
            _logger.Warn($"Cannot navigate forward to '{forwardPath}'. The directory doesn't exists.");
            return false;
        }

        if (tempPath == forwardPath)
        {
            _logger.Warn($"Cannot navigate forward to the same path: {tempPath}");
            return false;
        }

        _backwardStack.Push(tempPath);
        NavigateToInternal(forwardPath);

        _logger.Debug($"Successfully navigated forward to '{forwardPath}'.");
        return true;
    }

    public bool NavigateTo(VirtualPath targetPath)
    {
        ThrowHelper.Assert(targetPath.IsDirectory, "Target path is not representing a directory");

        _logger.Debug($"Navigating to '{targetPath}'");
        if (!CanNavigateTo(targetPath))
        {
            _logger.Warn($"Cannot navigate to '{targetPath}'.");
            return false;
        }

        _backwardStack.Push(CurrentPath);
        NavigateToInternal(targetPath);

        _logger.Debug($"Successfully navigated to '{targetPath}'");
        return true;
    }

    private void NavigateToInternal(VirtualPath path)
    {
        ThrowHelper.AssertNotNull(path, "The target path must be not null");
        ThrowHelper.Assert(path.IsDirectory, $"The target path must represent a directory: {path}");

        _crntPath = path;
        CurrentDirectory = _vfs.GetDirectory(_crntPath);
        CurrentPathChanged?.Invoke(this, path);
    }

    public IVirtualDirectory GetCurrentDirectory()
    {
        return _vfs.GetDirectory(CurrentPath);
    }

    public void Dispose()
    {
        _projectManager.ProjectOpened -= OnProjectOpened;
    }
}