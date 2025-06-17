using PokeLab.Domain;
using PokeCore.Logging;
using PokeCore.Diagnostics;
using PokeCore.IO;

namespace PokeLab.Application.ContentBrowser;

public sealed class ContentBrowserService(
    Logger<ContentBrowserService> logger,
    IContentBrowserCache cache,
    IContentBrowserNavigator navigator
) : IContentBrowserService
{
    public bool IsInitialized { get; private set; }

    public VirtualPath CurrentPath
    {
        get
        {
            AssertInitialized();
            return navigator.CurrentPath;
        }
    }

    public IReadOnlyCollection<IVirtualFile> Files => cache.Files;
    public IReadOnlyCollection<IVirtualDirectory> Directories => cache.Directories;

    public void Initialize(Project project)
    {
        ProjectVolume volume = project.GetDefaultVolume();
        VirtualPath path = VirtualPath.BuildRoot(volume.Scheme);

        navigator.Initialize(path);

        cache.Clear();
        cache.Refresh(ContentScope.All);

        IsInitialized = true;
        logger.Debug("Content browser service initialized");
    }

    public void NavigateBack()
    {
        if (navigator.CanGoBack())
        {
            navigator.GoBack();
            cache.Invalidate(ContentScope.CurrentDirectory);
        }
    }

    public void NavigateForward()
    {
        if (navigator.CanGoForward())
        {
            navigator.GoForward();
            cache.Invalidate(ContentScope.CurrentDirectory);
        }
    }

    public void NavigateTo(VirtualPath path)
    {
        if (navigator.CanGoTo(path))
        {
            navigator.GoTo(path);
            cache.Invalidate(ContentScope.CurrentDirectory);
        }
    }

    public void Refresh()
    {
        cache.Refresh(ContentScope.All);
    }

    public void Tick()
    {
        if (cache.HaveInvalidatedScopes)
        {
            cache.RefreshInvalidatedScopes();
        }
    }

    public void Reset()
    {
        navigator.Reset();
        cache.Clear();

        IsInitialized = false;
    }

    private void AssertInitialized()
    {
        ThrowHelper.Assert(IsInitialized, "The content browser needs to be initialized to perform this action");
    }
}
