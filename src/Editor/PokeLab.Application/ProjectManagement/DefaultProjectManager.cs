using PokeLab.Domain;
using PokeCore.Diagnostics;
using PokeCore.Logging;
using PokeEngine.Assets.VFS.Services;
using PokeEngine.Assets.VFS.Volumes;

namespace PokeLab.Application.ProjectManagement;

public sealed class DefaultProjectManager : IProjectManager
{
    public bool IsOpen => Current != null;
    public Project Current { get; private set; } = null!;

    private readonly Logger _logger;
    private readonly IProjectRepository _repository;
    private readonly IVirtualVolumeManager _volumeManager;

    public DefaultProjectManager(
        Logger<DefaultProjectManager> logger,
        IVirtualVolumeManager volumeManager,
        IProjectRepository repository
    )
    {
        _logger = logger;
        _repository = repository;
        _volumeManager = volumeManager;
    }

    public Task CloseAsync()
    {
        if (Current == null) return Task.CompletedTask;

        _logger.Debug("Closing currently opened project");
        foreach (ProjectVolume volume in Current.Volumes)
            _volumeManager.UnmountVolume(volume.Scheme);

        _logger.Info($"Previously opened project '{Current.Name}' closed.");
        Current = null!;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string basePath)
    {
        throw new NotImplementedException();
    }

    public async Task NewAsync(string name, string directoryPath)
    {
        ThrowHelper.AssertNotNullOrWhitespace(name);
        ThrowHelper.AssertNotNullOrWhitespace(directoryPath);

        ThrowHelper.Assert(Directory.Exists(directoryPath), $"Parameter '{nameof(directoryPath)}' needs to be an existing directory.");

        string rootPath = Path.Combine(directoryPath, name);
        _logger.Info($"Creating project '{name}' at path '{rootPath}'");
        if (Directory.Exists(rootPath))
        {
            _logger.Error($"Project with name '{name}' already exists in '{directoryPath}'");
            return;
        }

        var volumes = new List<ProjectVolume>
        {
            new ProjectVolume
            {
                Id = "local",
                Name = "Local",
                Scheme = "local",
                LogicalPath = Path.Combine(rootPath, "Content")
            },
            new ProjectVolume
            {
                Id = "library",
                Name = "Library",
                Scheme = "libs",
                LogicalPath = Path.Combine(rootPath, ".libs")
            },
        };

        Directory.CreateDirectory(rootPath);
        foreach (ProjectVolume volume in volumes)
            Directory.CreateDirectory(volume.LogicalPath);

        var project = new Project(name, rootPath, DateTime.Now, DateTime.Now, volumes);
        await _repository.SaveAsync(project);

        foreach (ProjectVolume volume in volumes)
        {
            IVirtualVolume virtualVolume = new PhysicalVolume(volume.Id, volume.Scheme, volume.Name, volume.LogicalPath);
            _volumeManager.MountVolume(virtualVolume);
        }

        Current = project;
        _logger.Info($"Project '{project.Name}' created.");
    }

    public async Task OpenAsync(string projectFilePath)
    {
        ThrowHelper.AssertNotNullOrWhitespace(projectFilePath, "The project file path cannot be null or whitespace");
        ThrowHelper.Assert(File.Exists(projectFilePath), "The project file path must exist");
        ThrowHelper.Assert(Path.GetExtension(projectFilePath) == ".pkproj", "The project file extension must be '.pkproj'");

        _logger.Info($"Opening project at path: '{projectFilePath}'");
        if (Current != null && Path.Combine(Current.RootPath, $"{Current.Name}.pkproj") == projectFilePath)
        {
            _logger.Warn($"Project '{Current.Name}' already opened.");
            return;
        }

        await CloseAsync();

        Project project = await _repository.LoadAsync(projectFilePath);

        string? openedProjectRootPath = Path.GetDirectoryName(projectFilePath);
        if (project.RootPath != openedProjectRootPath)
        {
            _logger.Warn($"Project location changed detected from '{project.RootPath}' to '{openedProjectRootPath ?? "<RootDir>"}'");

            if (openedProjectRootPath != null)
            {
                project.UpdateLocation(openedProjectRootPath);
                _logger.Debug("Project location updated");
                await _repository.SaveAsync(project);

                _logger.Debug("New project location saved");
            }
        }

        foreach (ProjectVolume volume in project.Volumes)
        {
            if (!Directory.Exists(volume.LogicalPath))
            {
                _logger.Warn($"Volume '{volume.Name}' wasn't found in the project root directory. Recreating.");
                Directory.CreateDirectory(volume.LogicalPath);
            }

            IVirtualVolume virtualVolume = new PhysicalVolume(volume.Id, volume.Scheme, volume.Name, volume.LogicalPath);
            _volumeManager.MountVolume(virtualVolume);
        }

        _logger.Info($"Project '{project.Name}' opened and configured.");
        Current = project;
    }

    public async Task SaveCurrentAsync()
    {
        if (!IsOpen)
        {
            _logger.Warn("Tried to save but no project is loaded.");
            return;
        }

        _logger.Info("Saving project");
        await _repository.SaveAsync(Current);
        _logger.Info($"Project '{Current.Name}' saved.");
    }
}