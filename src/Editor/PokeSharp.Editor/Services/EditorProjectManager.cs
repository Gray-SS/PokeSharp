using System.Diagnostics.CodeAnalysis;
using PokeSharp.Core.Logging;
using PokeSharp.Editor.Serializations;

namespace PokeSharp.Editor.Services;

public sealed class EditorProjectManager : IEditorProjectManager
{
    public bool HasActiveProject => ActiveProject != null;
    public EditorProject? ActiveProject { get; private set; }

    public event EventHandler<EditorProject>? ProjectOpened;

    private readonly ILogger _logger;

    public EditorProjectManager(ILogger logger)
    {
        _logger = logger;
    }

    public bool TryCreateProject(string projectName, string directoryPath, bool openOnCreation, [NotNullWhen(true)] out EditorProject? createdProject)
    {
        createdProject = null;
        try
        {
            _logger.Trace($"Creating project '{projectName}' at path '{directoryPath}'");
            if (!Directory.Exists(directoryPath))
            {
                _logger.Warn($"Unable to create the project at path '{directoryPath}' - The path must lead to an existing directory.");
                return false;
            }

            string projectPath = Path.Combine(directoryPath, projectName);
            createdProject = new EditorProject(projectName, projectPath);
            EditorProjectData data = createdProject.GetData();

            _logger.Trace($"Serializing newly created project");
            var serializer = new YamlDotNet.Serialization.SerializerBuilder().Build();
            var serializedProject = serializer.Serialize(data);

            _logger.Trace($"Creating directories");
            Directory.CreateDirectory(createdProject.ProjectDirPath);
            Directory.CreateDirectory(createdProject.ContentDirPath);

            _logger.Trace($"Writing serialized data to '{projectPath}'");
            using var writer = new StreamWriter(File.Create(createdProject.ProjectFilePath));
            writer.Write(serializedProject);

            _logger.Info($"Project successfully created at '{createdProject.ProjectDirPath}'.");
            return !openOnCreation || OpenProject(createdProject);
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
                _logger.Warn($"Unable to delete the project at path '{projectFile}' - The path must lead to an existing project file with extension '{EditorProject.ProjectExtension}'.");
                return false;
            }

            if (Path.GetExtension(projectFile) != EditorProject.ProjectExtension)
            {
                _logger.Warn($"Unable to delete the project at path '{projectFile}' - The file didn't have the required extension '{EditorProject.ProjectExtension}'.");
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

    public bool TryOpenProject(string fileProjectPath, [NotNullWhen(true)] out EditorProject? openedProject)
    {
        openedProject = null;
        try
        {
            _logger.Trace($"Opening project at path '{fileProjectPath}'");
            if (!File.Exists(fileProjectPath))
            {
                _logger.Warn($"Unable to open the project at path '{fileProjectPath}' - The path must lead to an existing project file with extension '{EditorProject.ProjectExtension}'.");
                return false;
            }

            if (Path.GetExtension(fileProjectPath) != EditorProject.ProjectExtension)
            {
                _logger.Warn($"Unable to open the project at path '{fileProjectPath}' - The file didn't have the required extension '{EditorProject.ProjectExtension}'.");
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
            var projectData = deserializer.Deserialize<EditorProjectData>(yaml);

            openedProject = EditorProject.FromData(projectPath, projectData);
            return OpenProject(openedProject);
        }
        catch (Exception ex)
        {
            _logger.Error($"Unexpected error occured while opening project - {ex.GetType().Name}: {ex.Message}");
            _logger.Debug($"{ex.StackTrace ?? "No stack trace available"}");
        }

        return false;
    }

    private bool OpenProject(EditorProject project)
    {
        if (ActiveProject != null && ActiveProject?.ProjectDirPath == project.ProjectDirPath)
        {
            _logger.Warn($"Tried to open a project that's already active.");
            return false;
        }

        ActiveProject = project;
        ProjectOpened?.Invoke(this, project);
        _logger.Info($"Project successfully opened from '{project.ProjectDirPath}'");
        return true;
    }
}