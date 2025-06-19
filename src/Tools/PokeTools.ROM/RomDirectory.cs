using System.Collections;
using PokeCore.IO;

namespace PokeTools.ROM;

public sealed class RomDirectory : RomEntry, IEnumerable<RomEntry>
{
    public IReadOnlyList<RomEntry> Entries => _entries;

    private readonly List<RomEntry> _entries;

    public RomDirectory(VirtualPath path) : base(path)
    {
        if (!path.IsDirectory)
            throw new InvalidOperationException($"The path '{path}' doesn't lead to a directory, make sure to add the '/' at the end");

        _entries = new List<RomEntry>();
    }

    public IEnumerable<RomFile> GetFiles()
    {
        return _entries.OfType<RomFile>();
    }

    public IEnumerable<RomDirectory> GetDirectories()
    {
        return _entries.OfType<RomDirectory>();
    }

    public RomDirectory AddDirectory(string dirName)
    {
        VirtualPath path = Path.Combine($"{dirName}/");
        if (_entries.Any(x => x.Path == path))
            throw new InvalidOperationException($"Directory with name '{dirName}' already exists at path: '{path}'");

        var directory = new RomDirectory(path);
        _entries.Add(directory);

        return directory;
    }

    public RomFile AddFile(string filename, ReadOnlyMemory<byte> data)
    {
        VirtualPath path = Path.Combine(filename);
        if (_entries.Any(x => x.Path == path))
            throw new InvalidOperationException($"File with name '{filename}' already exists at path: '{path}'");

        var file = new RomFile(path, data);
        _entries.Add(file);

        return file;
    }

    public IEnumerator<RomEntry> GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}