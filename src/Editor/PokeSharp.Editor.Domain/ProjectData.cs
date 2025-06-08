namespace PokeSharp.Editor.Domain;

public sealed class ProjectData
{
    public string Name { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }

    public ProjectData(string name, DateTime createdAt, DateTime updatedAt)
    {
        Name = name;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}