using PokeEngine.Assets.VFS;

namespace PokeLab.Application.ContentBrowser;

public sealed class ContentPathChangedArgs : EventArgs
{
    public VirtualPath NewPath { get; }
    public VirtualPath OldPath { get; }

    public IContentBrowserNavigator Navigator { get; }

    public ContentPathChangedArgs(IContentBrowserNavigator navigator, VirtualPath newPath, VirtualPath oldPath)
    {
        Navigator = navigator;
        NewPath = newPath;
        OldPath = oldPath;
    }
}

public interface IContentBrowserNavigator
{
    VirtualPath CurrentPath { get; }
    IVirtualDirectory CurrentDirectory { get; }

    public event EventHandler<ContentPathChangedArgs>? OnPathChanged;

    bool CanGoBack();
    bool CanGoForward();
    bool CanGoTo(VirtualPath path);

    void GoTo(VirtualPath path);
    void GoBack();
    void GoForward();

    void Initialize(VirtualPath path);
}