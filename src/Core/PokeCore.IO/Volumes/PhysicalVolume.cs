using PokeCore.Diagnostics;
using PokeCore.Logging;
using PokeCore.IO.Events;

namespace PokeCore.IO.Volumes;

public sealed class PhysicalVolume : BaseVirtualVolume, IReadableVolume, IWritableVolume, IWatchableVolume
{
    public string PhysicalPath { get; }

    public event EventHandler<FileSystemChangedArgs>? OnFileSystemChanged;

    private readonly Logger _logger = null!;
    private readonly FileSystemWatcher _watcher = null!;

    /// Used because the file system watcher can't provide informations about
    /// whether a deleted path was a file or a directory. This sucks.
    private readonly Dictionary<string, bool> _pathIsDirectory = null!;

    public PhysicalVolume(string id, string scheme, string displayName, string physicalPath, bool enableWatcher = true) : base(id, scheme, displayName)
    {
        if (!Directory.Exists(physicalPath))
            throw new ArgumentException($"Unable to create physical volume - Physical path '{physicalPath}' doesn't exists or is not a directory");

        PhysicalPath = Path.GetFullPath(physicalPath);

        _logger = LoggerFactory.GetLogger<PhysicalVolume>();
        _logger.Trace($"Volume '{displayName}' is watching at path '{physicalPath}'");

        if (enableWatcher)
        {
            _watcher = new FileSystemWatcher(PhysicalPath)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite
            };

            _pathIsDirectory = new Dictionary<string, bool>();
            PopulateIsDirectoryCache(PhysicalPath);

            _watcher.Changed += HandleOnFileChanged;
            _watcher.Created += HandleOnFileChanged;
            _watcher.Deleted += HandleOnFileChanged;
            _watcher.Renamed += HandleOnFileChanged;
            _watcher.Error += HandleFileSystemWatcherError;
        }
    }

    private void PopulateIsDirectoryCache(string directory)
    {
        foreach (var dir in Directory.GetDirectories(directory, "*", SearchOption.AllDirectories))
            _pathIsDirectory[dir] = true;

        foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
            _pathIsDirectory[file] = false;
    }

    private void HandleFileSystemWatcherError(object sender, ErrorEventArgs e)
    {
        Exception? exception = e.GetException();
        if (exception == null)
            _logger.Error("Unknown error occured in physical file system watcher");
        else
        {
            _logger.Error($"Error occured in physical file system watcher: {exception.GetType().Name}: {exception.Message}");
            _logger.Debug($"{exception.StackTrace ?? "No stack trace available"}");
        }
    }

    private void HandleOnFileChanged(object sender, FileSystemEventArgs e)
    {
        var changeType = e.ChangeType switch
        {
            WatcherChangeTypes.Created => FileSystemChangeType.Created,
            WatcherChangeTypes.Changed => FileSystemChangeType.Modified,
            WatcherChangeTypes.Renamed => FileSystemChangeType.Renamed,
            WatcherChangeTypes.Deleted => FileSystemChangeType.Deleted,
            _ => throw new NotImplementedException($"The change type '{e.ChangeType}' was not handled.")
        };

        string fullPath = e is RenamedEventArgs renamed ? renamed.FullPath : e.FullPath;
        string relativePath = Path.GetRelativePath(PhysicalPath, fullPath);

        bool isDirectory;
        if (e.ChangeType == WatcherChangeTypes.Deleted)
        {
            if (!_pathIsDirectory.TryGetValue(fullPath, out isDirectory))
            {
                _logger.Warn($"Unknown type for deleted path {fullPath}, assuming file.");
                isDirectory = false;
            }

            _pathIsDirectory.Remove(fullPath);
        }
        else
        {
            isDirectory = Directory.Exists(fullPath);
            _pathIsDirectory[fullPath] = isDirectory;
        }

        string localPath = relativePath + (isDirectory ? "/" : "");
        VirtualPath virtualPath = RootPath.Combine(localPath);

        ThrowHelper.Assert(!(isDirectory && !virtualPath.IsDirectory), $"Path was cached as a directory but the virtual path represents a file: '{virtualPath}'.");
        OnFileSystemChanged?.Invoke(this, new FileSystemChangedArgs(changeType, virtualPath));
    }

    private string GetPhysicalPath(VirtualPath virtualPath)
    {
        if (virtualPath.IsRoot) return PhysicalPath;

        string path = virtualPath.LocalPath.Replace('/', Path.DirectorySeparatorChar);
        return Path.Combine(PhysicalPath, path);
    }

    public override bool EntryExists(VirtualPath path)
    {
        string physicalPath = GetPhysicalPath(path);
        return File.Exists(physicalPath) || Directory.Exists(physicalPath);
    }

    public override bool FileExists(VirtualPath path)
    {
        string physicalPath = GetPhysicalPath(path);
        return File.Exists(physicalPath);
    }

    public override bool DirectoryExists(VirtualPath path)
    {
        string physicalPath = GetPhysicalPath(path);
        return Directory.Exists(physicalPath);
    }

    public override IVirtualFile GetFile(VirtualPath virtualPath)
    {
        if (virtualPath.IsDirectory)
            throw new InvalidOperationException($"The provided path doesn't lead to a file: '{virtualPath}'");

        return new VirtualFile(this, virtualPath);
    }

    public override IVirtualDirectory GetDirectory(VirtualPath virtualPath)
    {
        if (!virtualPath.IsDirectory)
            throw new InvalidOperationException($"The provided path doesn't lead to a directory: '{virtualPath}'");

        return new VirtualDirectory(this, virtualPath);
    }

    public override IEnumerable<IVirtualFile> GetFiles(VirtualPath virtualPath)
    {
        IVirtualDirectory foundDirectory = GetDirectory(virtualPath);
        if (!foundDirectory.Exists)
            yield break;

        string physicalPath = GetPhysicalPath(virtualPath);
        foreach (string filePath in Directory.GetFiles(physicalPath))
        {
            string fileName = Path.GetFileName(filePath);
            yield return foundDirectory.GetFile(fileName);
        }
    }

    public override IEnumerable<IVirtualDirectory> GetDirectories(VirtualPath virtualPath)
    {
        IVirtualDirectory foundDirectory = GetDirectory(virtualPath);
        if (!foundDirectory.Exists)
            yield break;

        string physicalPath = GetPhysicalPath(virtualPath);
        foreach (string directoryPath in Directory.GetDirectories(physicalPath))
        {
            string directoryName = Path.GetFileName(directoryPath);
            yield return foundDirectory.GetDirectory(directoryName);
        }
    }

    public IVirtualFile CreateFile(VirtualPath virtualPath, bool overwrite)
    {
        ThrowHelper.Assert(virtualPath.IsFile, $"Cannot create a file at a path representing a directory: '{virtualPath}'");

        string physicalPath = GetPhysicalPath(virtualPath);
        _logger.Trace($"Creating file at '{virtualPath}'");

        if (!overwrite && (File.Exists(physicalPath) || Directory.Exists(physicalPath)))
            throw new InvalidOperationException($"Entry already exists at path '{virtualPath}' and overwrite is disabled");

        // Ensure parent directory exists
        string? parentPhysicalPath = Path.GetDirectoryName(physicalPath);
        if (!string.IsNullOrEmpty(parentPhysicalPath))
        {
            Directory.CreateDirectory(parentPhysicalPath);
        }

        using var _ = File.Create(physicalPath);

        _logger.Trace($"File created at '{virtualPath}'.");
        return new VirtualFile(this, virtualPath);
    }

    public IVirtualDirectory CreateDirectory(VirtualPath virtualPath)
    {
        ThrowHelper.Assert(virtualPath.IsDirectory, $"Cannot to create a directory at a path representing a file: '{virtualPath}'");

        _logger.Trace($"Creating directory at '{virtualPath}'");
        string physicalPath = GetPhysicalPath(virtualPath);

        if (File.Exists(physicalPath))
            throw new InvalidOperationException($"A file already exists at path '{virtualPath}'");

        Directory.CreateDirectory(physicalPath);

        _logger.Trace($"Directory created at '{virtualPath}'.");
        return new VirtualDirectory(this, virtualPath);
    }

    public IVirtualEntry MoveEntry(VirtualPath srcPath, VirtualPath destDirectoryPath)
    {
        ThrowHelper.Assert(destDirectoryPath.IsDirectory, $"Cannot move an entry at a destination path representing a file: '{destDirectoryPath}'");

        string physicalSrcPath = GetPhysicalPath(srcPath);
        string physicalDestDirPath = GetPhysicalPath(destDirectoryPath);

        if (srcPath.IsDirectory)
        {
            if (!Directory.Exists(physicalSrcPath))
                throw new DirectoryNotFoundException($"No directory found at '{srcPath}'");

            _logger.Trace($"Moving directory from '{srcPath}' to '{destDirectoryPath}'");
            string targetDir = Path.Combine(physicalDestDirPath, Path.GetFileName(physicalSrcPath.TrimEnd('/')));
            Directory.Move(physicalSrcPath, targetDir);

            _logger.Trace($"Directory moved from '{srcPath}' to '{destDirectoryPath}'.");
            return new VirtualDirectory(this, destDirectoryPath.Combine(Path.GetFileName(physicalSrcPath.TrimEnd('/')) + '/'));
        }
        else
        {
            if (!File.Exists(physicalSrcPath))
                throw new FileNotFoundException($"No file found at '{srcPath}'");

            _logger.Trace($"Moving file from '{srcPath}' to '{destDirectoryPath}'");
            string targetFile = Path.Combine(physicalDestDirPath, Path.GetFileName(physicalSrcPath));

            File.Move(physicalSrcPath, targetFile);

            _logger.Trace($"File moved from '{srcPath}' to '{destDirectoryPath}'");
            return new VirtualFile(this, destDirectoryPath.Combine(Path.GetFileName(physicalSrcPath)));
        }
    }

    public bool DeleteEntry(VirtualPath virtualPath)
    {
        string physicalPath = GetPhysicalPath(virtualPath);

        string entryTypeName = virtualPath.IsFile ? "file" : "directory";
        _logger.Trace($"Deleting {entryTypeName} at '{virtualPath}'");
        if (!File.Exists(physicalPath) && !Directory.Exists(physicalPath))
        {
            _logger.Warn($"{entryTypeName} doesn't exists. Skipping.");
            return false; // Entry doesn't exist, consider it as already deleted
        }

        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
        }
        else if (Directory.Exists(physicalPath))
        {
            Directory.Delete(physicalPath, recursive: true);
        }

        _logger.Trace($"{entryTypeName} deleted at '{virtualPath}'");
        return true;
    }

    public IVirtualEntry RenameEntry(VirtualPath virtualPath, string newName)
    {
        ThrowHelper.AssertNotNullOrWhitespace(newName, "The new name cannot be null or empty");

        string physicalPath = GetPhysicalPath(virtualPath);

        string entryTypeName = virtualPath.IsFile ? "file" : "directory";
        _logger.Trace($"Renaming {entryTypeName} from '{virtualPath.Name}' to '{newName}' at '{virtualPath}'");
        if (!File.Exists(physicalPath) && !Directory.Exists(physicalPath))
            throw new FileNotFoundException($"No file or directory found at virtual path '{virtualPath}'");

        VirtualPath parentPath = virtualPath.GetParent();
        string parentPhysicalPath = GetPhysicalPath(parentPath);
        string newPhysicalPath = Path.Combine(parentPhysicalPath, newName);

        if (File.Exists(newPhysicalPath) || Directory.Exists(newPhysicalPath))
            throw new InvalidOperationException($"An entry with name '{newName}' already exists");

        if (File.Exists(physicalPath))
        {
            File.Move(physicalPath, newPhysicalPath);
            VirtualPath newVirtualPath = parentPath.Combine(newName);

            _logger.Trace($"file renamed from '{virtualPath.Name}' to '{newName}' at '{virtualPath}'");
            return new VirtualFile(this, newVirtualPath);
        }
        else if (Directory.Exists(physicalPath))
        {
            Directory.Move(physicalPath, newPhysicalPath);
            VirtualPath newVirtualPath = parentPath.Combine(newName + "/");

            _logger.Trace($"Directory renamed from '{virtualPath.Name}' to '{newName}' at '{virtualPath}'");
            return new VirtualDirectory(this, newVirtualPath);
        }

        throw new FileNotFoundException($"No file or directory found at virtual path '{virtualPath}'");
    }

    public IVirtualEntry DuplicateEntry(VirtualPath virtualPath)
    {
        string physicalPath = GetPhysicalPath(virtualPath);

        string entryTypeName = virtualPath.IsFile ? "file" : "directory";
        _logger.Trace($"Duplicating {entryTypeName} at '{virtualPath}'");
        if (!File.Exists(physicalPath) && !Directory.Exists(physicalPath))
            throw new FileNotFoundException($"No file or directory found at virtual path '{virtualPath}'");

        VirtualPath parentPath = virtualPath.GetParent();
        string parentPhysicalPath = GetPhysicalPath(parentPath);

        string originalName = virtualPath.IsDirectory
            ? virtualPath.Name.TrimEnd('/')
            : virtualPath.Name;

        string duplicatedName = GenerateUniqueName(parentPhysicalPath, originalName);
        string duplicatePhysicalPath = Path.Combine(parentPhysicalPath, duplicatedName);

        if (File.Exists(physicalPath))
        {
            File.Copy(physicalPath, duplicatePhysicalPath);
            VirtualPath duplicateVirtualPath = parentPath.Combine(duplicatedName);

            _logger.Trace($"File duplicated at '{virtualPath}'");
            return new VirtualFile(this, duplicateVirtualPath);
        }
        else if (Directory.Exists(physicalPath))
        {
            CopyDirectoryRecursive(physicalPath, duplicatePhysicalPath);
            VirtualPath duplicateVirtualPath = parentPath.Combine(duplicatedName + "/");

            _logger.Trace($"Directory duplicated at '{virtualPath}'");
            return new VirtualDirectory(this, duplicateVirtualPath);
        }

        throw new FileNotFoundException($"No file or directory found at virtual path '{virtualPath}'");
    }

    public Stream OpenWrite(VirtualPath virtualPath)
    {
        ThrowHelper.Assert(virtualPath.IsFile, $"Cannot open directory for writing: '{virtualPath}'");

        string physicalPath = GetPhysicalPath(virtualPath);

        if (!File.Exists(physicalPath))
            throw new FileNotFoundException($"File at virtual path '{virtualPath}' doesn't exist");

        return File.OpenWrite(physicalPath);
    }

    public StreamWriter OpenWriter(VirtualPath virtualPath)
    {
        return new StreamWriter(OpenWrite(virtualPath));
    }

    public Stream OpenRead(VirtualPath virtualPath)
    {
        ThrowHelper.Assert(virtualPath.IsFile, $"Cannot open directory for reading: '{virtualPath}'.");

        string physicalPath = GetPhysicalPath(virtualPath);

        if (!File.Exists(physicalPath))
            throw new FileNotFoundException($"File at virtual path '{virtualPath}' doesn't exist");

        return File.OpenRead(physicalPath);
    }

    public byte[] ReadBytes(VirtualPath virtualPath)
    {
        ThrowHelper.Assert(virtualPath.IsFile, $"Cannot read bytes from a directory: '{virtualPath}'");

        string physicalPath = GetPhysicalPath(virtualPath);
        if (!File.Exists(physicalPath))
            throw new FileNotFoundException($"File at virtual path '{virtualPath}' doesn't exist");

        return File.ReadAllBytes(physicalPath);
    }

    private static void CopyDirectoryRecursive(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);

        // Copy files
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            string destFile = Path.Combine(destDir, Path.GetFileName(file));
            File.Copy(file, destFile);
        }

        // Copy subdirectories recursively
        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            string destSubDir = Path.Combine(destDir, Path.GetFileName(dir));
            CopyDirectoryRecursive(dir, destSubDir);
        }
    }

    private static string GenerateUniqueName(string directory, string baseName)
    {
        string nameWithoutExt = Path.GetFileNameWithoutExtension(baseName);
        string ext = Path.GetExtension(baseName);
        int count = 1;

        string candidate = baseName;
        while (File.Exists(Path.Combine(directory, candidate)) ||
               Directory.Exists(Path.Combine(directory, candidate)))
        {
            candidate = $"{nameWithoutExt} ({count++}){ext}";
        }

        return candidate;
    }

    public void Dispose()
    {
        _watcher?.Dispose();
    }
}