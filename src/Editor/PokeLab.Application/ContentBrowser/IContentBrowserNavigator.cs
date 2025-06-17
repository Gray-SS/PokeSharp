using PokeCore.IO;

namespace PokeLab.Application.ContentBrowser;

public interface IContentBrowserNavigator
{
    bool IsInitialized { get; }
    VirtualPath CurrentPath { get; }
    IVirtualDirectory CurrentDirectory { get; }

    bool CanGoBack();
    void GoBack();

    bool CanGoForward();
    void GoForward();

    bool CanGoTo(VirtualPath path);
    void GoTo(VirtualPath path);

    void Initialize(VirtualPath path);
    void Reset();
}