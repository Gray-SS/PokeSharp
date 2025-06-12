namespace PokeLab.Domain;

public sealed class ProjectData
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string RootPath { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ProjectVolume> Volumes { get; set; }

    // Parameterless constructor for serialization
    public ProjectData()
    {
        Id = Guid.Empty;
        Name = string.Empty;
        RootPath = string.Empty;
        CreatedAt = DateTime.MinValue;
        UpdatedAt = DateTime.MinValue;
        Volumes = [];
    }

    public ProjectData(Guid id, string name, string rootPath, DateTime createdAt, DateTime updatedAt, List<ProjectVolume> volumes)
    {
        Name = name;
        RootPath = rootPath;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Volumes = volumes;
    }
}