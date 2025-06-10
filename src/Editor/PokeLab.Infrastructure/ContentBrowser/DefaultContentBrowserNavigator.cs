using PokeEngine.Assets.VFS;
using PokeEngine.Assets.VFS.Services;
using PokeCore.Hosting;
using PokeCore.Hosting.Logging;
using PokeLab.Application.ContentBrowser;

namespace PokeLab.Infrastructure.ContentBrowser;

public sealed class DefaultContentBrowserNavigator : IContentBrowserNavigator
{
    public VirtualPath CurrentPath { get; private set; }
    public IVirtualDirectory CurrentDirectory { get; private set; }

    public event EventHandler<ContentPathChangedArgs>? OnPathChanged;

    private readonly Logger _logger;
    private readonly IVirtualFileSystem _vfs;
    private readonly Stack<VirtualPath> _backwardStack;
    private readonly Stack<VirtualPath> _forwardStack;

    public DefaultContentBrowserNavigator(IVirtualFileSystem vfs, Logger logger)
    {
        CurrentPath = null!;
        CurrentDirectory = null!;

        _vfs = vfs;
        _logger = logger;
        _forwardStack = new Stack<VirtualPath>();
        _backwardStack = new Stack<VirtualPath>();
    }

    public void Initialize(VirtualPath virtualPath)
    {
        GoToInternal(virtualPath);
    }

    public bool CanGoBack()
    {
        return _backwardStack.Count > 0;
    }

    public bool CanGoForward()
    {
        return _forwardStack.Count > 0;
    }

    public bool CanGoTo(VirtualPath path)
    {
        return CurrentPath != path && _vfs.DirectoryExists(path);
    }

    public void GoBack()
    {
        _logger.Debug("Navigating back");
        if (!CanGoBack())
        {
            _logger.Warn("Cannot navigate back.");
            return;
        }

        VirtualPath tempPath = CurrentPath;
        VirtualPath backwardPath = _backwardStack.Pop();

        ThrowHelper.Assert(backwardPath.IsDirectory, "Backward path must be a directory");
        if (!_vfs.DirectoryExists(backwardPath))
        {
            _logger.Warn($"Cannot navigate back to '{backwardPath}'. The directory doesn't exists.");
            return;
        }

        if (backwardPath == tempPath)
        {
            _logger.Warn($"Cannot navigate back to the same path: {tempPath}");
            return;
        }

        _forwardStack.Push(tempPath);
        GoToInternal(backwardPath);

        _logger.Debug($"Successfully navigated back to '{backwardPath}'.");
    }

    public void GoForward()
    {
    }

    public void GoTo(VirtualPath path)
    {
    }

    private void GoToInternal(VirtualPath path)
    {
        VirtualPath oldPath = CurrentPath;
        CurrentPath = path;
        CurrentDirectory = _vfs.GetDirectory(CurrentPath);
        OnPathChanged?.Invoke(this, new ContentPathChangedArgs(this, path, oldPath));
    }
}