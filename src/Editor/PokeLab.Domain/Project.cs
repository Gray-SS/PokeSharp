namespace PokeLab.Domain;

public sealed class Project
{
    public Guid Id { get; }
    public string Name { get; }
    public string RootPath { get; private set; }
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

    public void UpdateLocation(string rootPath)
    {
        RootPath = Path.GetFullPath(rootPath);

        foreach (ProjectVolume volume in _volumes)
        {
            string newLogicalPath = Path.Combine(rootPath, Path.GetFileName(volume.LogicalPath));
            volume.UpdateLocation(newLogicalPath);
        }
    }

    public static Project FromData(ProjectData data)
    {
        return new Project(data.Name, data.RootPath, data.CreatedAt, data.UpdatedAt, data.Volumes);
    }

    public ProjectData GetData()
    {
        return new ProjectData(Id, Name, RootPath, CreatedAt, UpdatedAt, _volumes);
    }
}