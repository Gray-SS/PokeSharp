using System.Diagnostics;

namespace PokeSharp.Assets.VFS;

/// <summary>
/// Represents a virtual file path with a scheme and normalized path, useful for referencing logical assets
/// in a virtual file system (e.g. <c>file://textures/player.png</c>).
/// </summary>
public sealed class VirtualPath : IEquatable<VirtualPath>
{
    /// <summary>
    /// Gets the name of the current virtual path. If it's a file, returns the filename. If it's a directory, returns the directory name.
    /// </summary>
    public string Name => GetName();

    /// <summary>
    /// Gets the file extension of the current virtual path. If it'a file, returns the extension. If it's a directory, returns empty.
    /// </summary>
    public string Extension => GetFileExtension();

    /// <summary>
    /// Gets a value indicating whether this virtual path points to a directory.
    /// </summary>
    public bool IsDirectory { get; }

    /// <summary>
    /// Gets the scheme of the virtual path.
    /// </summary>
    /// <remarks>
    /// Common schemes are: <c>file</c>, <c>http</c>, <c>ftp</c>, etc.
    /// </remarks>
    public string Scheme { get; }

    /// <summary>
    /// Gets a value indicating whether this virtual path is the root of the scheme.
    /// </summary>
    /// <remarks>
    /// For example, <c>file://</c> is a root path; <c>file://image.png</c> is not.
    /// </remarks>
    public bool IsRoot => string.IsNullOrEmpty(LocalPath);

    /// <summary>
    /// Gets the normalized virtual path part (excluding the scheme).
    /// </summary>
    public string LocalPath { get; }

    /// <summary>
    /// Gets the full URI representation of the virtual path (e.g. <c>file://assets/image.png</c>).
    /// </summary>
    public string Uri { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualPath"/> class.
    /// </summary>
    /// <param name="scheme">The URI scheme (e.g. <c>file</c>).</param>
    /// <param name="path">The virtual path string (can contain slashes, <c>..</c>, etc.).</param>
    public VirtualPath(string scheme, string path)
    {
        Scheme = scheme;
        LocalPath = NormalizePath(path);
        Uri = $"{Scheme}://{LocalPath}";
        IsDirectory = IsRoot || path.EndsWith('/');

        Debug.Assert(LocalPath != null, "Local path was null");
    }

    /// <summary>
    /// Returns the parent virtual path of this path.
    /// </summary>
    /// <returns>
    /// A new <see cref="VirtualPath"/> that is the parent directory, or <c>null</c> if this is the root path.
    /// </returns>
    public VirtualPath GetParent()
    {
        if (IsRoot) return null!;

        var parts = LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length <= 1)
            return Root(Scheme);

        string parentPath = string.Join('/', parts, 0, parts.Length - 1) + '/';
        return new VirtualPath(Scheme, parentPath);
    }

    /// <summary>
    /// Combines this path with a relative subpath, producing a new <see cref="VirtualPath"/>.
    /// </summary>
    /// <param name="subPath">The subpath to append (can be a file or directory).</param>
    /// <returns>A new combined <see cref="VirtualPath"/> instance.</returns>
    public VirtualPath Combine(string subPath)
    {
        if (string.IsNullOrEmpty(subPath))
            return this;

        string combined = IsRoot ? subPath : $"{LocalPath.TrimEnd('/')}/{subPath.TrimStart('/')}";
        return new VirtualPath(Scheme, combined);
    }

    public VirtualPath AddExtension(string extension)
    {
        if (string.IsNullOrEmpty(extension))
            return this;

        if (!extension.StartsWith('.'))
            extension += '.';

        return new VirtualPath(Scheme, LocalPath + extension);
    }

    /// <summary>
    /// Creates a new root virtual path for the given scheme.
    /// </summary>
    /// <param name="scheme">The URI scheme (e.g. <c>file</c>).</param>
    /// <returns>A <see cref="VirtualPath"/> representing the root path for the scheme.</returns>
    public static VirtualPath Root(string scheme)
    {
        return new VirtualPath(scheme, string.Empty);
    }

    /// <summary>
    /// Parses a full URI string into a <see cref="VirtualPath"/>.
    /// </summary>
    /// <param name="uri">A string in the format <c>scheme://path</c>.</param>
    /// <returns>A <see cref="VirtualPath"/> instance.</returns>
    /// <exception cref="FormatException">Thrown if the URI is not in a valid format.</exception>
    public static VirtualPath Parse(string uri)
    {
        string[] splited = uri.Split("://", 2, StringSplitOptions.None);
        if (splited.Length != 2)
            throw new FormatException($"The uri '{uri}' wasn't in the valid format. Expected format: {{scheme}}://{{virtualPath}}");

        string scheme = splited[0];
        string path = splited[1];
        return new VirtualPath(scheme, path);
    }

    /// <summary>
    /// Gets the last segment of the path, representing the file or folder name.
    /// </summary>
    /// <returns>The final segment of the path.</returns>
    private string GetName()
    {
        if (IsRoot) return string.Empty;

        var parts = LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return parts[^1];
    }

    private string GetFileExtension()
    {
        if (IsRoot) return string.Empty;

        var ext = LocalPath.Split('.');
        return ext.Length > 0 ? "." + ext[^1] : string.Empty;
    }

    /// <summary>
    /// Normalizes a virtual path by removing redundant separators, ".", and ".." segments.
    /// </summary>
    /// <param name="path">The virtual path to normalize.</param>
    /// <returns>A clean, normalized path string.</returns>
    private static string NormalizePath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;

        bool isDirectory = path.EndsWith('/');
        path = path.Replace('\\', '/');

        var segments = path.Split('/')
                           .Where(p => !string.IsNullOrWhiteSpace(p) && p != ".")
                           .ToList();

        Stack<string> stack = new();
        foreach (var segment in segments)
        {
            if (segment == ".." && stack.Count > 0)
                stack.Pop();
            else if (segment != "..")
                stack.Push(segment);
        }

        return string.Join("/", stack.Reverse()) + (isDirectory ? "/" : string.Empty);
    }

    /// <summary>
    /// Returns the string representation of the virtual path (i.e. the URI).
    /// </summary>
    /// <returns>The full virtual URI.</returns>
    public override string ToString()
    {
        return Uri;
    }

    public bool Equals(VirtualPath? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return string.Equals(Scheme, other.Scheme, StringComparison.OrdinalIgnoreCase)
            && string.Equals(LocalPath, other.LocalPath, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj)
    {
        return obj is VirtualPath other && Equals(other);
    }

    public static bool operator ==(VirtualPath? left, VirtualPath? right)
    {
        if (left is null) return right is null;

        return left.Equals(right);
    }

    public static bool operator !=(VirtualPath? left, VirtualPath? right)
    {
        return !Equals(left, right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            StringComparer.OrdinalIgnoreCase.GetHashCode(Scheme),
            StringComparer.Ordinal.GetHashCode(LocalPath)
        );
    }
}
