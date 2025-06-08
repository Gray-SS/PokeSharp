namespace PokeSharp.Editor.Domain;

public sealed class ProjectVolume
{
    public string Name { get; }
    public string LogicalPath { get; }

    public ProjectVolume(string name, string logicalPath)
    {
        Name = name;
        LogicalPath = logicalPath;
    }
}