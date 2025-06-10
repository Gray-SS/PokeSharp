using System.Diagnostics;

namespace PokeEngine.Assets.VFS;

/// <summary>
/// Represents a virtual file path consisting of a scheme and a normalized path,
/// used to reference logical assets in a virtual file system.
/// </summary>
/// <remarks>
/// A <see cref="VirtualPath"/> can represent either a directory or a file.
/// Directory paths must end with a trailing '/'. <br/> <br/>
///
/// Paths in this form cannot be used directly to perform file system operations.
/// To interact with the virtual file system (e.g., check existence, delete, rename, etc.),
/// use <see cref="IVirtualFileSystem"/> or one of the interfaces
/// <see cref="IVirtualFile"/> / <see cref="IVirtualDirectory"/>.
/// </remarks>
public sealed class VirtualPath : IEquatable<VirtualPath>
{
    /// <summary>
    /// Gets the name of this path.
    /// </summary>
    /// <remarks>
    /// If the path represents a file, returns the file name including its extension.
    /// If the path represents a directory, returns the directory name in the path.
    /// </remarks>
    /// <example>
    /// <code>
    /// var directoryPath = VirtualPath.Parse("local://Assets/Textures/");
    /// var filePath = VirtualPath.Parse("local://Assets/Textures/image.png");
    /// var dirName = directoryPath.Name;   // "Textures"
    /// var fileName = filePath.Name;       // "image.png"
    /// </code>
    /// </example>
    public string Name => GetName();

    /// <summary>
    /// Gets the file extension of this path.
    /// </summary>
    /// <remarks>
    /// <b>Note:</b> It is assumed that the current path represents a file.
    /// A <c>Debug.Assert</c> is used to enforce this assumption during development.
    /// </remarks>
    public string Extension => GetFileExtension();

    /// <summary>
    /// Indicates whether this path represents a directory (ends with a slash).
    /// </summary>
    /// <remarks>
    /// This assumes that directories end with a trailing '/' and files do not.
    /// </remarks>
    public bool IsDirectory { get; }

    /// <summary>
    /// Indicates whether this path represents a file (does not end with a slash).
    /// </summary>
    /// <remarks>
    /// This assumes that directories end with a trailing '/' and files do not.
    /// </remarks>
    public bool IsFile => !IsDirectory;

    // TODO: Move the non-related scheme docs
    /// <summary>
    /// Gets the scheme component of this virtual path (e.g. <c>local</c>, <c>pokefirered</c>).
    /// </summary>
    /// <remarks>
    /// The scheme indicates the logical volume or source for the path. <br/><br/>
    /// <b>Note:</b> In a <i>runtime context</i>, some schemes (like <c>local</c> and <c>libs</c>) are statically available. While in the <i>editor context</i>, since we're working with different environment/project those schemes may only be available when a project is loaded, as they map to project-specific content or metadata directories. <br/> <br/>
    /// <b>Note for the future:</b> To enable the editor to adopt a more runtime-like approach we could create an editor launcher that opens a project and allows those volumes to be mounted directly at the start of the editor.
    /// </remarks>
    /// <example>
    /// var path = VirtualPath.Parse("local://Assets/Textures/image.png");
    /// var scheme = path.Scheme; // "local"
    /// </example>
    public string Scheme { get; }

    /// <summary>
    /// Indicates whether this path is a root path (e.g. <c>local://</c>)
    /// </summary>
    /// <remarks>
    /// A root path is defined as one that does not contain a local path component.
    /// </remarks>
    /// <example>
    /// <code>
    /// var texturesPath = VirtualPath.Parse("local://Assets/Textures/");
    /// var localPath = VirtualPath.Parse("local://");
    /// var isTexturesPathRoot = texturesPath.IsRoot;   // false
    /// var isLocalPathRoot = localPath.IsRoot;         // true
    /// </code>
    /// </example>
    public bool IsRoot => string.IsNullOrEmpty(LocalPath);

    /// <summary>
    /// Gets the normalized virtual path part (excluding the scheme).
    /// </summary>
    /// <remarks>
    /// This path is normalized to use forward slashes and does not include the URI scheme (e.g. <c>Assets/Textures/image.png</c>).
    /// </remarks>
    public string LocalPath { get; }

    /// <summary>
    /// Gets the full URI representation of this virtual path, including the scheme and local path.
    /// </summary>
    /// <remarks>
    /// For example: <c>local://Assets/Textures/image.png</c>.
    /// </remarks>
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
        IsDirectory = IsRoot || LocalPath.EndsWith('/');

        Debug.Assert(LocalPath != null, "Local path was null");
    }

    /// <summary>
    /// Returns the parent path of this path.
    /// </summary>
    /// <returns>
    /// A new <see cref="VirtualPath"/> instance that represents the parent directory of this path.
    /// </returns>
    /// <remarks>
    /// <b>Note:</b> It is assumed that the current path does not represent a root path.
    /// A <c>Debug.Assert</c> is used to enforce this assumption during development.
    /// </remarks>
    public VirtualPath GetParent()
    {
        Debug.Assert(!IsRoot);

        var parts = LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length <= 1)
            return BuildRoot(Scheme);

        string parentPath = string.Join('/', parts, 0, parts.Length - 1) + '/';
        return new VirtualPath(Scheme, parentPath);
    }

    /// <summary>
    /// Combines this path with a relative subpath, producing a new <see cref="VirtualPath"/>.
    /// </summary>
    /// <param name="subPath">The relative subpath to append. Can represent a file or a directory.</param>
    /// <returns>A new combined <see cref="VirtualPath"/> instance.</returns>
    /// <remarks>
    /// To append a directory, make sure the <paramref name="subPath"/> ends with a '/' character.
    /// For example: <c>myPath.Combine("Assets/")</c> produces a directory path, while
    /// <c>myPath.Combine("file.txt")</c> produces a file path.
    /// <br/> <br/>
    /// <b>Note:</b> It is assumed that the current path represents a directory.
    /// A <c>Debug.Assert</c> is used to enforce this assumption during development.
    /// </remarks>
    public VirtualPath Combine(string subPath)
    {
        Debug.Assert(IsDirectory, "Current path must represent a directory to combine a sub path");

        if (string.IsNullOrEmpty(subPath))
            return this;

        string combined = IsRoot ? subPath : $"{LocalPath.TrimEnd('/')}/{subPath.TrimStart('/')}";
        return new VirtualPath(Scheme, combined);
    }

    /// <summary>
    /// Determines if the specified path is a sub path of this directory, no matter the depth.
    /// </summary>
    /// <param name="path">The path of the entry to determine.</param>
    /// <returns><c>true</c> if the path is a sub path of this directory; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// Also, this method <b>does not check</b> if the specified path <b>exists</b> in the file system.
    /// It only performs a syntactic checks based on the URI structure. Use <see cref="IVirtualDirectory.EntryExists(string)"/> or <see cref="Services.IVirtualFileSystem"/> to determine existence of a path.
    /// <br/><br/>
    /// <b>Note:</b> It is assumed that the current path represents a directory.
    /// A <c>Debug.Assert</c> is used to enforce this assumption during development.
    /// </remarks>
    public bool IsParentOf(VirtualPath path)
    {
        Debug.Assert(IsDirectory, "Current path must represent a directory to determine children.");

        if (Uri == path.Uri) return false;
        if (path.Scheme != Scheme) return false;
        if (IsRoot && !path.IsRoot) return true;

        var thisSegments = Uri.Trim('/').Split('/');
        var childSegments = path.Uri.Trim('/').Split('/');

        if (thisSegments.Length >= childSegments.Length)
            return false;

        for (int i = 0; i < thisSegments.Length; i++)
        {
            if (!string.Equals(thisSegments[i], childSegments[i], StringComparison.Ordinal))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Determines if the specified path is a <b>direct</b> sub path of this directory.
    /// </summary>
    /// <param name="path">The path of the entry to determine.</param>
    /// <returns><c>true</c> if the path is a direct sub path of this directory; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// This method <b>does not check</b> if the specified path <b>exists</b> in the file system.
    /// It only performs a syntactic checks based on the URI structure. Use <see cref="IVirtualDirectory.EntryExists(string)"/> or <see cref="Services.IVirtualFileSystem"/> to determine existence of a path.
    /// <br/><br/>
    /// <b>Note:</b> It is assumed that the current path represents a directory.
    /// A <c>Debug.Assert</c> is used to enforce this assumption during development.
    /// </remarks>
    public bool IsDirectParentOf(VirtualPath path)
    {
        Debug.Assert(IsDirectory, "Current path must represent a directory to determine children.");

        if (Uri == path.Uri) return false;
        if (path.Scheme != Scheme) return false;
        if (IsRoot && !path.IsRoot) return true;

        var thisSegments = Uri.Trim('/').Split('/');
        var childSegments = path.Uri.Trim('/').Split('/');

        if (thisSegments.Length + 1 != childSegments.Length)
            return false;

        for (int i = 0; i < thisSegments.Length; i++)
        {
            if (!string.Equals(thisSegments[i], childSegments[i], StringComparison.Ordinal))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Creates a new <see cref="VirtualPath"/> with the specified file extension.
    /// </summary>
    /// <param name="extension">The file extension to append (e.g. <c>.bin</c>, <c>.txt</c>, <c>.png</c>).</param>
    /// <returns>A new <see cref="VirtualPath"/> instance with the specified file extension.</returns>
    /// <remarks>
    /// <b>Note:</b> It is assumed that the current path represents a file.
    /// A <c>Debug.Assert</c> is used to enforce this assumption during development.
    /// </remarks>
    public VirtualPath AddExtension(string extension)
    {
        Debug.Assert(IsFile, "Current path must represent a file to append an extension to it.");

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
    public static VirtualPath BuildRoot(string scheme)
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

    /// <summary>
    /// Gets the file extension of this path.
    /// </summary>
    /// <remarks>
    /// <b>Note:</b> It is assumed that the current path represents a file.
    /// A <c>Debug.Assert</c> is used to enforce this assumption during development.
    /// </remarks>
    private string GetFileExtension()
    {
        Debug.Assert(IsFile, "The current path must represent a file to get it's file extension.");

        int lastSlash = LocalPath.LastIndexOf('/');
        int lastDot = LocalPath.LastIndexOf('.');

        if (lastDot > lastSlash)
        {
            return LocalPath[lastDot..];
        }

        return string.Empty;
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
        return StringComparer.Ordinal.GetHashCode(Uri);
    }
}
