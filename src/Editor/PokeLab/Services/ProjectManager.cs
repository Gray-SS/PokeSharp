using System.Diagnostics.CodeAnalysis;
using PokeEngine.Assets;
using PokeCore.IO.Services;
using PokeCore.IO.Volumes;
using PokeCore.Hosting.Logging;
using PokeLab.Serializations;

namespace PokeLab.Services;

public sealed class ProjectManager : IProjectManager
{
    private const string LibsPath = ".libs";
    private const string ContentPath = "Content";

    public bool HasActiveProject => ActiveProject != null;
    public Project? ActiveProject { get; private set; }

    public event EventHandler<Project>? ProjectOpened;

    private readonly Logger _logger;
    private readonly IVirtualVolumeManager _volumeManager;
    private readonly AssetPipeline _assetPipeline;

    public ProjectManager(
        Logger logger,
        IVirtualVolumeManager volumeManager,
        AssetPipeline assetPipeline)
    {
        _logger = logger;
        _volumeManager = volumeManager;
        _assetPipeline = assetPipeline;
    }

    public bool TryCreateProject(string projectName, string directoryPath, bool openOnCreation, [NotNullWhen(true)] out Project? project)
    {
        project = null;
        try
        {
            _logger.Trace($"Creating project '{projectName}' at path '{directoryPath}'");
            if (!Directory.Exists(directoryPath))
            {
                _logger.Warn($"Unable to create the project at path '{directoryPath}' - The path must lead to an existing directory.");
                return false;
            }
            _logger.Trace($"Creating directories");

            string projectPath = Path.Combine(directoryPath, projectName);
            string libsPath = Path.Combine(projectPath, LibsPath);
            string contentPath = Path.Combine(projectPath, ContentPath);

            Directory.CreateDirectory(projectPath);
            Directory.CreateDirectory(libsPath);
            Directory.CreateDirectory(contentPath);

            _volumeManager.UnmountAll();

            var libsVolume = new PhysicalVolume("library", "libs", "Library", libsPath);
            _volumeManager.MountVolume(libsVolume);

            var contentVolume = new PhysicalVolume("assets", "local", "Assets", contentPath);
            _volumeManager.MountVolume(contentVolume);

            project = new Project(projectName, projectPath, libsVolume, contentVolume);
            ProjectData data = project.GetData();

            _logger.Trace($"Serializing newly created project");
            var serializer = new YamlDotNet.Serialization.SerializerBuilder().Build();
            var serializedProject = serializer.Serialize(data);

            _logger.Trace($"Writing serialized data to '{projectPath}'");
            using var writer = new StreamWriter(File.Create(project.ProjectFile));
            writer.Write(serializedProject);

            _logger.Info($"Project successfully created at '{project.ProjectRoot}'.");
            return !openOnCreation || OpenProject(project);
        }
        catch (Exception ex)
        {
            _logger.Error($"Unexpected error occured while creating project - {ex.GetType().Name}: {ex.Message}");
            _logger.Debug($"{ex.StackTrace ?? "No stack trace available"}");
        }

        return false;
    }

    public bool TryDeleteProject(string projectFile)
    {
        try
        {
            _logger.Trace($"Deleting project at path '{projectFile}'");
            if (!File.Exists(projectFile))
            {
                _logger.Warn($"Unable to delete the project at path '{projectFile}' - The path must lead to an existing project file with extension '{Project.ProjectExtension}'.");
                return false;
            }

            if (Path.GetExtension(projectFile) != Project.ProjectExtension)
            {
                _logger.Warn($"Unable to delete the project at path '{projectFile}' - The file didn't have the required extension '{Project.ProjectExtension}'.");
                return false;
            }

            string? projectPath = Path.GetDirectoryName(projectFile);
            if (projectPath == null)
            {
                _logger.Warn($"Unable to delete the project at path '{projectFile}' - The provided project file isn't contained in a directory");
                return false;
            }

            _logger.Trace($"Deleting project directory");
            Directory.Delete(projectPath, true);

            _logger.Trace($"Project successfully deteted at path '{projectPath}'");
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error($"Unexpected error occured while deleting project - {ex.GetType().Name}: {ex.Message}");
            _logger.Debug($"{ex.StackTrace ?? "No stack trace available"}");
        }

        return false;
    }

    public bool TryOpenProject(string fileProjectPath, [NotNullWhen(true)] out Project? projet)
    {
        projet = null;
        try
        {
            _logger.Trace($"Opening project at path '{fileProjectPath}'");
            if (!File.Exists(fileProjectPath))
            {
                _logger.Warn($"Unable to open the project at path '{fileProjectPath}' - The path must lead to an existing project file with extension '{Project.ProjectExtension}'.");
                return false;
            }

            if (Path.GetExtension(fileProjectPath) != Project.ProjectExtension)
            {
                _logger.Warn($"Unable to open the project at path '{fileProjectPath}' - The file didn't have the required extension '{Project.ProjectExtension}'.");
                return false;
            }

            string? projectPath = Path.GetDirectoryName(fileProjectPath);
            if (projectPath == null)
            {
                _logger.Warn($"Unable to open the project at path '{fileProjectPath}' - The provided project file isn't contained in a directory");
                return false;
            }

            _logger.Trace($"Reading project file content");
            string yaml = File.ReadAllText(fileProjectPath);

            _logger.Trace($"Deserializing project data");
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().Build();
            var projectData = deserializer.Deserialize<ProjectData>(yaml);

            string libsPath = Path.Combine(projectPath, LibsPath);
            string contentPath = Path.Combine(projectPath, ContentPath);

            if (!Directory.Exists(contentPath))
            {
                _logger.Warn($"Content root path wasn't found, re-creating");
                Directory.CreateDirectory(contentPath);
            }

            if (!Directory.Exists(libsPath))
            {
                _logger.Warn($"Library root path wasn't found, re-creating");
                Directory.CreateDirectory(libsPath);
            }

            _volumeManager.UnmountAll();

            var libsVolume = new PhysicalVolume("library", "libs", "Library", libsPath);
            _volumeManager.MountVolume(libsVolume);

            var contentVolume = new PhysicalVolume("local", "local", "Assets", contentPath);
            _volumeManager.MountVolume(contentVolume);

            projet = new Project(projectData.Name, projectPath, libsVolume, contentVolume);
            return OpenProject(projet);
        }
        catch (Exception ex)
        {
            _logger.Error($"Unexpected error occured while opening project - {ex.GetType().Name}: {ex.Message}");
            _logger.Debug($"{ex.StackTrace ?? "No stack trace available"}");
        }

        return false;
    }

    private bool OpenProject(Project project)
    {
        if (ActiveProject != null && ActiveProject?.ProjectRoot == project.ProjectRoot)
        {
            _logger.Warn($"Tried to open a project that's already active.");
            return false;
        }

        ActiveProject = project;
        ProjectOpened?.Invoke(this, project);

        // ThreadHelper.RunOnMainThread(_assetPipeline.ReimportAll);
        _assetPipeline.ReimportAll();
        _logger.Info($"Project successfully opened from '{project.ProjectRoot}'");
        return true;
    }
}