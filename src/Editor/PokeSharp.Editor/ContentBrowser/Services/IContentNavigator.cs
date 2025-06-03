using PokeSharp.Assets.VFS;

namespace PokeSharp.Editor.ContentBrowser.Services;

public interface IContentNavigator
{
    VirtualPath CurrentPath { get; set; }
    IVirtualDirectory CurrentDirectory { get; }

    // TODO: Create a specific event args for this
    event EventHandler<VirtualPath>? CurrentPathChanged;

    bool CanNavigateBack();
    bool CanNavigateForward();
    bool CanNavigateTo(VirtualPath path);

    bool NavigateTo(VirtualPath path);
    bool NavigateBack();
    bool NavigateForward();

    IVirtualDirectory GetCurrentDirectory();
}