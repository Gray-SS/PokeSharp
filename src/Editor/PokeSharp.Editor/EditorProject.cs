using PokeSharp.Editor.Serializations;

namespace PokeSharp.Editor;

public sealed class EditorProject
{
    public const string ProjectExtension = ".pkproj";

    public string Name { get; private set; }
    public string ProjectDirPath { get; private set; }
    public string ProjectFilePath => Path.Combine(ProjectDirPath, $"{Name.ToLower()}{ProjectExtension}");
    public string ContentDirPath => Path.Combine(ProjectDirPath, "Content");
    public string BuildPath => Path.Combine(ProjectDirPath, "Build");

    public DateTime CreatedAt { get; private set; }
    public DateTime LastOpened { get; set; }

    public EditorProject(string name, string projectPath)
    {
        Name = name;
        ProjectDirPath = projectPath;
        CreatedAt = DateTime.Now;
        LastOpened = DateTime.Now;
    }

    public EditorProjectData GetData()
    {
        return new EditorProjectData
        {
            Name = Name
        };
    }

    public static EditorProject FromData(string projectPath, EditorProjectData data)
    {
        return new EditorProject(data.Name, projectPath);
    }
}