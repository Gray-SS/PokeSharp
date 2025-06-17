using PokeLab.Domain;
using PokeCore.IO;

namespace PokeLab.Application.ContentBrowser;

public interface IContentBrowserService
{
    bool IsInitialized { get; }

    VirtualPath CurrentPath { get; }
    IReadOnlyCollection<IVirtualFile> Files { get; }
    IReadOnlyCollection<IVirtualDirectory> Directories { get; }

    void Initialize(Project project);

    void Tick();
    void Refresh();

    void NavigateTo(VirtualPath path);
    void NavigateBack();
    void NavigateForward();

    void Reset();
}