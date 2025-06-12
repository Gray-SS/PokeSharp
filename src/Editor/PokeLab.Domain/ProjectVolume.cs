namespace PokeLab.Domain;

public sealed class ProjectVolume
{
    public string Id { get; set; }
    public string Scheme { get; set; }
    public string Name { get; set; }
    public string LogicalPath { get; set; }

    public ProjectVolume()
    {
        Id = string.Empty;
        Scheme = string.Empty;
        Name = string.Empty;
        LogicalPath = string.Empty;
    }

    public ProjectVolume(string id, string scheme, string name, string logicalPath)
    {
        Id = id;
        Name = name;
        Scheme = scheme;
        LogicalPath = logicalPath;
    }

    public void UpdateLocation(string logicalPath)
    {
        LogicalPath = logicalPath;
    }
}