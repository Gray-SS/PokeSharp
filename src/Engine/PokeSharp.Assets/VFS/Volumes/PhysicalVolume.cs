using PokeSharp.Assets.VFS.Events;

namespace PokeSharp.Assets.VFS.Volumes;

public sealed class PhysicalVolume : BaseVirtualVolume, IReadableVolume, IWritableVolume, IWatchableVolume
{
    public string PhysicalPath { get; }

    public event EventHandler<FileSystemChangedArgs>? OnFileSystemChanged;

    private readonly FileSystemWatcher _watcher;

    public PhysicalVolume(string id, string scheme, string displayName, string physicalPath) : base(id, scheme, displayName)
    {
        if (!Directory.Exists(physicalPath))
            throw new ArgumentException($"Unable to create physical volume - Physical path '{physicalPath}' doesn't exists or is not a directory");

        PhysicalPath = Path.GetFullPath(physicalPath);
        _watcher = new FileSystemWatcher(PhysicalPath)
        {
            IncludeSubdirectories = true,
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite
        };

        _watcher.Changed += HandleOnFileChanged;
        _watcher.Created += HandleOnFileChanged;
        _watcher.Deleted += HandleOnFileChanged;
        _watcher.Renamed += HandleOnFileChanged;
    }

    private void HandleOnFileChanged(object sender, FileSystemEventArgs e)
    {
        var changeType = e.ChangeType switch
        {
            WatcherChangeTypes.Created => FileSystemChangedArgs.ChangeType.Created,
            WatcherChangeTypes.Changed => FileSystemChangedArgs.ChangeType.Modified,
            WatcherChangeTypes.Renamed => FileSystemChangedArgs.ChangeType.Renamed,
            WatcherChangeTypes.Deleted => FileSystemChangedArgs.ChangeType.Deleted,
            _ => throw new NotImplementedException($"The change type '{e.ChangeType}' was not handled.")
        };

        string fullPath = e is RenamedEventArgs renamed ? renamed.FullPath : e.FullPath;
        VirtualPath virtualPath = RootPath.Combine(Path.GetRelativePath(PhysicalPath, fullPath));

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
        if (virtualPath.IsDirectory)
            throw new InvalidOperationException($"Cannot create file at directory path: '{virtualPath}'");

        string physicalPath = GetPhysicalPath(virtualPath);

        if (!overwrite && (File.Exists(physicalPath) || Directory.Exists(physicalPath)))
            throw new InvalidOperationException($"Entry already exists at path '{virtualPath}' and overwrite is disabled");

        // Ensure parent directory exists
        string? parentPhysicalPath = Path.GetDirectoryName(physicalPath);
        if (!string.IsNullOrEmpty(parentPhysicalPath))
        {
            Directory.CreateDirectory(parentPhysicalPath);
        }

        using var _ = File.Create(physicalPath);

        return new VirtualFile(this, virtualPath);
    }

    public IVirtualDirectory CreateDirectory(VirtualPath virtualPath)
    {
        if (!virtualPath.IsDirectory)
            throw new InvalidOperationException($"Cannot create directory at file path: '{virtualPath}'");

        string physicalPath = GetPhysicalPath(virtualPath);

        if (File.Exists(physicalPath))
            throw new InvalidOperationException($"A file already exists at path '{virtualPath}'");

        Directory.CreateDirectory(physicalPath);

        return new VirtualDirectory(this, virtualPath);
    }

    public IVirtualEntry MoveEntry(VirtualPath srcPath, VirtualPath destDirectoryPath)
    {
        if (!destDirectoryPath.IsDirectory)
            throw new InvalidOperationException($"Destination '{destDirectoryPath}' is not a directory.");

        string physicalSrcPath = GetPhysicalPath(srcPath);
        string physicalDestDirPath = GetPhysicalPath(destDirectoryPath);

        if (srcPath.IsDirectory)
        {
            if (!Directory.Exists(physicalSrcPath))
                throw new DirectoryNotFoundException($"No directory found at '{srcPath}'");

            string targetDir = Path.Combine(physicalDestDirPath, Path.GetFileName(physicalSrcPath.TrimEnd('/')));
            Directory.Move(physicalSrcPath, targetDir);
            return new VirtualDirectory(this, destDirectoryPath.Combine(Path.GetFileName(physicalSrcPath.TrimEnd('/')) + '/'));
        }
        else
        {
            if (!File.Exists(physicalSrcPath))
                throw new FileNotFoundException($"No file found at '{srcPath}'");

            string targetFile = Path.Combine(physicalDestDirPath, Path.GetFileName(physicalSrcPath));
            System.Console.WriteLine($"Moving physical '{physicalSrcPath}' to '{physicalDestDirPath}'");

            File.Move(physicalSrcPath, targetFile);
            return new VirtualFile(this, destDirectoryPath.Combine(Path.GetFileName(physicalSrcPath)));
        }
    }

    public bool DeleteEntry(VirtualPath virtualPath)
    {
        string physicalPath = GetPhysicalPath(virtualPath);

        if (!File.Exists(physicalPath) && !Directory.Exists(physicalPath))
            return false; // Entry doesn't exist, consider it as already deleted

        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
        }
        else if (Directory.Exists(physicalPath))
        {
            Directory.Delete(physicalPath, recursive: true);
        }

        return true;
    }

    public IVirtualEntry RenameEntry(VirtualPath virtualPath, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("New name cannot be null or empty", nameof(newName));

        string physicalPath = GetPhysicalPath(virtualPath);

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
            return new VirtualFile(this, newVirtualPath);
        }
        else if (Directory.Exists(physicalPath))
        {
            Directory.Move(physicalPath, newPhysicalPath);
            VirtualPath newVirtualPath = parentPath.Combine(newName + "/");
            return new VirtualDirectory(this, newVirtualPath);
        }

        throw new FileNotFoundException($"No file or directory found at virtual path '{virtualPath}'");
    }

    public IVirtualEntry DuplicateEntry(VirtualPath virtualPath)
    {
        string physicalPath = GetPhysicalPath(virtualPath);

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
            return new VirtualFile(this, duplicateVirtualPath);
        }
        else if (Directory.Exists(physicalPath))
        {
            CopyDirectoryRecursive(physicalPath, duplicatePhysicalPath);
            VirtualPath duplicateVirtualPath = parentPath.Combine(duplicatedName + "/");
            return new VirtualDirectory(this, duplicateVirtualPath);
        }

        throw new FileNotFoundException($"No file or directory found at virtual path '{virtualPath}'");
    }

    public Stream OpenWrite(VirtualPath virtualPath)
    {
        if (virtualPath.IsDirectory)
            throw new InvalidOperationException($"Cannot open directory for writing: '{virtualPath}'");

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
        if (virtualPath.IsDirectory)
            throw new InvalidOperationException($"Cannot open directory for reading: '{virtualPath}'");

        string physicalPath = GetPhysicalPath(virtualPath);

        if (!File.Exists(physicalPath))
            throw new FileNotFoundException($"File at virtual path '{virtualPath}' doesn't exist");

        return File.OpenRead(physicalPath);
    }

    public byte[] ReadBytes(VirtualPath virtualPath)
    {
        if (virtualPath.IsDirectory)
            throw new InvalidOperationException($"Cannot read bytes from a directory: '{virtualPath}'");

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