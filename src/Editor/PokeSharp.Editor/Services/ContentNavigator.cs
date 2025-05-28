using PokeSharp.Assets.VFS;

namespace PokeSharp.Editor.Services;

public sealed class ContentNavigator : IContentNavigator
{
    public VirtualPath CurrentPath
    {
        get => _crntPath;
        set
        {
            if (_crntPath == value || value == null) return;

            _crntPath = value;
            CurrentPathChanged?.Invoke(this, value);
        }
    }

    private VirtualPath _crntPath;
    private readonly IVirtualFileSystem _vfs;
    private readonly Stack<VirtualPath> _backwardStack;
    private readonly Stack<VirtualPath> _forwardStack;

    public event EventHandler<VirtualPath>? CurrentPathChanged;

    public ContentNavigator(IVirtualFileSystem vfs)
    {
        _vfs = vfs;
        _crntPath = null!;
        _forwardStack = new Stack<VirtualPath>();
        _backwardStack = new Stack<VirtualPath>();
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
        return _vfs.Exists(path);
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
}