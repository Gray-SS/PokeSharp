using PokeSharp.Assets.VFS;
using PokeSharp.Assets.VFS.Volumes;
using PokeSharp.Editor.ContentBrowser.Events;

namespace PokeSharp.Editor.ContentBrowser.Services;

/// <summary>
/// Provides caching functionality for the Content Browser.
/// </summary>
public interface IContentCacheService
{
    /// <summary>
    /// Triggered when the cache is refreshed.
    /// </summary>
    event EventHandler<ContentCacheRefreshedArgs>? OnCacheRefreshed;

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
    /// Marks a specific content scope as invalidated.
    /// </summary>
    /// <param name="scope">The content scope to invalidate. Defaults to all content.</param>
    /// <remarks>
    /// Invalidation is deferred and will be processed later.
    /// For immediate update, use <see cref="Refresh"/>.
    /// </remarks>
    void Invalidate(ContentScope scope);

    /// <summary>
    /// Marks a specific file or directory at the given path as invalidated.
    /// </summary>
    /// <param name="entryPath">The path of the file or directory to invalidate.</param>
    /// <remarks>
    /// Invalidation is deferred and will be processed later.
    /// For immediate update, use <see cref="Refresh"/>.
    /// </remarks>
    // void Invalidate(VirtualPath entryPath);

    /// <summary>
    /// Immediately refreshes a specific content scope.
    /// </summary>
    /// <param name="scope">The content scope to refresh. Defaults to all content.</param>
    /// <remarks>
    /// Use <see cref="Invalidate"/> for deferred refresh.
    /// </remarks>
    void Refresh(ContentScope scope);

    /// <summary>
    /// Immediately refreshes a specific file or directory at the given path.
    /// </summary>
    /// <param name="entryPath">The path of the file or directory to refresh.</param>
    /// <remarks>
    /// Use <see cref="Invalidate"/> for deferred refresh.
    /// </remarks>
    // void Refresh(VirtualPath entryPath);
}
