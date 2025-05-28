using PokeSharp.Assets.VFS;

namespace PokeSharp.Editor.Services;

public interface IContentNavigator
{
    VirtualPath CurrentPath { get; }

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