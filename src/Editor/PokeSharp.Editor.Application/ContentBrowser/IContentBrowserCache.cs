using PokeSharp.Assets.VFS;
using PokeSharp.Assets.VFS.Volumes;

namespace PokeSharp.Editor.Application.ContentBrowser;

public sealed class ContentBrowserCacheRefreshed : EventArgs
{
    public ContentScope Scope { get; }

    public ContentBrowserCacheRefreshed(ContentScope scope)
    {
        Scope = scope;
    }
}

/// <summary>
/// Provides caching functionality for the Content Browser.
/// </summary>
public interface IContentBrowserCache
{
    /// <summary>
    /// Triggered when the cache is refreshed.
    /// </summary>
    event EventHandler<ContentBrowserCacheRefreshed>? OnRefresh;

    /// <summary>
    /// Gets all cached files.
    /// </summary>
    /// <returns>A read-only collection of all cached files.</returns>
    IReadOnlyCollection<IVirtualFile> Files { get; }

    /// <summary>
    /// Gets all cached volumes.
    /// </summary>
    /// <returns>A read-only collection of cached volumes.</returns>
    IReadOnlyCollection<IVirtualVolume> Volumes { get; }

    /// <summary>
    /// Gets all cached directories.
    /// </summary>
    /// <returns>A read-only collection of all cached directories.</returns>
    IReadOnlyCollection<IVirtualDirectory> Directories { get; }

    /// <summary>
    /// Determines if the cache have invalidated scope(s) waiting to be refreshed.
    /// </summary>
    /// <returns><c>true</c> if the cache have invalidated scope(s); <c>false</c>, otherwise</returns>
    bool HaveInvalidatedScopes { get; }

    /// <summary>
    /// Marks a specific content scope as invalidated.
    /// </summary>
    /// <param name="scope">The content scope to invalidate. Defaults to all content.</param>
    /// <remarks>
    /// Invalidation is deferred and will be processed later.
    /// For immediate update, use <see cref="Refresh"/>.
    /// </remarks>
    void Invalidate(ContentScope scope);

    /// <summary>
    /// Immediately refreshes a specific content scope.
    /// </summary>
    /// <param name="scope">The content scope to refresh. Defaults to all content.</param>
    /// <remarks>
    /// Use <see cref="Invalidate"/> for deferred refresh.
    /// </remarks>
    void Refresh(ContentScope scope);

    /// <summary>
    /// Immediately refreshed all invalidated scopes.
    /// </summary>
    void RefreshInvalidatedScopes();
}
