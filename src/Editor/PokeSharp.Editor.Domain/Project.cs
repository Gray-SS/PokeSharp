namespace PokeSharp.Editor.Domain;

public sealed class Project
{
    public string Name { get; }
    public string RootPath { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyCollection<ProjectVolume> Volumes => _volumes;

    private readonly List<ProjectVolume> _volumes;

    public Project(string name, string rootPath, DateTime createdAt, DateTime updatedAt, List<ProjectVolume> volumes)
    {
        Name = name;
        RootPath = rootPath;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;

        _volumes = volumes;
    }

    public void Touch()
    {
        UpdatedAt = DateTime.Now;
    }

    public ProjectData GetData()
    {
        return new ProjectData(Name, CreatedAt, UpdatedAt);
    }
}