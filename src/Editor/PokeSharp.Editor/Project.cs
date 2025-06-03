using PokeSharp.Assets.VFS.Volumes;
using PokeSharp.Core;
using PokeSharp.Editor.Serializations;
using PokeSharp.Editor.Services;

namespace PokeSharp.Editor;

public sealed class Project
{
    public const string ProjectExtension = ".pkproj";

    public static bool IsActive => _projectManager.HasActiveProject;
    public static Project Active => _projectManager.ActiveProject!;

    public string Name { get; private set; }
    public string ProjectRoot { get; private set; }
    public string ProjectFile => Path.Combine(ProjectRoot, $"{Name.ToLower()}{ProjectExtension}");
    public string LibsRoot => LibsVolume.PhysicalPath;
    public string AssetsRoot => AssetsVolume.PhysicalPath;
    public string BuildRoot => Path.Combine(ProjectRoot, "Build");

    public PhysicalVolume LibsVolume { get; }
    public PhysicalVolume AssetsVolume { get; }

    public DateTime CreatedAt { get; private set; }
    public DateTime LastOpened { get; set; }

    private static readonly IProjectManager _projectManager = ServiceLocator.GetRequiredService<IProjectManager>();

    public Project(string name, string projectPath, PhysicalVolume libsVolume, PhysicalVolume assetsVolume)
    {
        Name = name;
        LibsVolume = libsVolume;
        AssetsVolume = assetsVolume;
        ProjectRoot = projectPath;
        CreatedAt = DateTime.Now;
        LastOpened = DateTime.Now;
    }

    public ProjectData GetData()
    {
        return new ProjectData
        {
            Name = Name
        };
    }
}