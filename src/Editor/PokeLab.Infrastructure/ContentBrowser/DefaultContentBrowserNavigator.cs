using PokeCore.Diagnostics;
using PokeCore.Logging;
using PokeCore.IO;
using PokeCore.IO.Services;
using PokeLab.Application.ContentBrowser;

namespace PokeLab.Infrastructure.ContentBrowser;

public sealed class DefaultContentBrowserNavigator(
    Logger<DefaultContentBrowserNavigator> logger,
    IVirtualFileSystem vfs
) : IContentBrowserNavigator
{
    public bool IsInitialized { get; }
    public VirtualPath CurrentPath { get; private set; } = null!;
    public IVirtualDirectory CurrentDirectory { get; private set; } = null!;

    private readonly Stack<VirtualPath> _backwardStack = new();
    private readonly Stack<VirtualPath> _forwardStack = new();

    public void Initialize(VirtualPath virtualPath)
    {
        GoToInternal(virtualPath);
        logger.Debug($"Navigator initialized at path: '{virtualPath}'");
    }

    public void Reset()
    {
        CurrentPath = null!;
        CurrentDirectory = null!;
        logger.Debug("Navigator reset");
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
        return CurrentPath != path && vfs.DirectoryExists(path);
    }

    public void GoBack()
    {
        logger.Debug("Navigating back");
        if (!CanGoBack())
        {
            logger.Warn("Cannot navigate back.");
            return;
        }

        VirtualPath tempPath = CurrentPath;
        VirtualPath backwardPath = _backwardStack.Pop();

        ThrowHelper.Assert(backwardPath.IsDirectory, "Backward path must be a directory");
        if (!vfs.DirectoryExists(backwardPath))
        {
            logger.Warn($"Cannot navigate back to '{backwardPath}'. The directory doesn't exists.");
            return;
        }

        if (backwardPath == tempPath)
        {
            logger.Warn($"Cannot navigate back to the same path: {tempPath}");
            return;
        }

        _forwardStack.Push(tempPath);
        GoToInternal(backwardPath);

        logger.Debug($"Successfully navigated back to '{backwardPath}'.");
    }

    public void GoForward()
    {
    }

    public void GoTo(VirtualPath path)
    {
    }

    private void GoToInternal(VirtualPath path)
    {
        CurrentPath = path;
        CurrentDirectory = vfs.GetDirectory(CurrentPath);
    }
}