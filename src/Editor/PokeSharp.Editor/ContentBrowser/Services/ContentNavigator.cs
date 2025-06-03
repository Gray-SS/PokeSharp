using PokeSharp.Assets.VFS;
using PokeSharp.Assets.VFS.Services;
using PokeSharp.Editor.Services;

namespace PokeSharp.Editor.ContentBrowser.Services;

public sealed class ContentNavigator : IContentNavigator, IDisposable
{
    public VirtualPath CurrentPath
    {
        get => _crntPath;
        set
        {
            if (_crntPath == value || value == null) return;

            _crntPath = value;
            CurrentDirectory = _vfs.GetDirectory(_crntPath);
            CurrentPathChanged?.Invoke(this, value);
        }
    }

    public IVirtualDirectory CurrentDirectory { get; private set; }

    private VirtualPath _crntPath;
    private readonly IVirtualFileSystem _vfs;
    private readonly Stack<VirtualPath> _backwardStack;
    private readonly Stack<VirtualPath> _forwardStack;
    private readonly IProjectManager _projectManager;

    public event EventHandler<VirtualPath>? CurrentPathChanged;

    public ContentNavigator(IVirtualFileSystem vfs, IProjectManager projectManager)
    {
        CurrentDirectory = null!;

        _vfs = vfs;
        _crntPath = null!;
        _forwardStack = new Stack<VirtualPath>();
        _backwardStack = new Stack<VirtualPath>();

        _projectManager = projectManager;
        _projectManager.ProjectOpened += OnProjectOpened;
    }

    private void OnProjectOpened(object? sender, Project e)
    {
        CurrentPath = e.AssetsVolume.RootPath;
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
        return _vfs.DirectoryExists(path);
    }

    public bool NavigateBack()
    {
        if (!CanNavigateBack()) return false;

        _forwardStack.Push(CurrentPath);
        CurrentPath = _backwardStack.Pop();
        return true;
    }

    public bool NavigateForward()
    {
        if (!CanNavigateForward()) return false;

        _backwardStack.Push(CurrentPath);
        CurrentPath = _forwardStack.Pop();
        return true;
    }

    public bool NavigateTo(VirtualPath path)
    {
        if (!CanNavigateTo(path)) return false;

        _backwardStack.Push(CurrentPath);

        CurrentPath = path;
        return true;
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