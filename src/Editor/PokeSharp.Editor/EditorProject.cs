using PokeSharp.Editor.Serializations;

namespace PokeSharp.Editor;

public sealed class EditorProject
{
    public const string ProjectExtension = ".pkproj";

    public string Name { get; private set; }
    public string ProjectRoot { get; private set; }
    public string ProjectFile => Path.Combine(ProjectRoot, $"{Name.ToLower()}{ProjectExtension}");
    public string ContentRoot => Path.Combine(ProjectRoot, "Content");
    public string BuildRoot => Path.Combine(ProjectRoot, "Build");
    public string LibsRoot => Path.Combine(ProjectRoot, ".libs");

    public DateTime CreatedAt { get; private set; }
    public DateTime LastOpened { get; set; }

    public EditorProject(string name, string projectPath)
    {
        Name = name;
        ProjectRoot = projectPath;
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