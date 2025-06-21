using PokeLab.Domain;
using PokeCore.Diagnostics;
using PokeCore.Logging;
using PokeCore.IO.Services;
using PokeCore.IO.Volumes;

namespace PokeLab.Application.ProjectManagement;

public sealed class ProjectService(
    Logger<ProjectService> logger,
    IVirtualVolumeManager volumeManager,
    IProjectRepository projectRepository
) : IProjectService
{
    public bool IsOpen => Current != null;
    public Project Current { get; private set; } = null!;

    public Task<Result<Project>> CloseAsync()
    {
        if (Current == null)
        {
            string errorMessage = "Couln't close the project. No project was loaded";
            logger.Warn(errorMessage);
            return Task.FromResult(Result<Project>.Failed(errorMessage));
        }

        logger.Debug("Closing currently opened project");
        foreach (ProjectVolume volume in Current.Volumes)
            volumeManager.UnmountVolume(volume.Scheme);

        logger.Info($"Previously opened project '{Current.Name}' closed.");

        Project closedProject = Current;
        Current = null!;

        var result = Result<Project>.Success(closedProject);
        return Task.FromResult(result);
    }

    public async Task<Result<Project>> CreateAsync(string name, string directoryPath)
    {
        ThrowHelper.AssertNotNullOrWhitespace(name);
        ThrowHelper.AssertNotNullOrWhitespace(directoryPath);

        ThrowHelper.Assert(Directory.Exists(directoryPath), $"Parameter '{nameof(directoryPath)}' needs to be an existing directory.");

        string rootPath = Path.Combine(directoryPath, name);
        logger.Info($"Creating project '{name}' at path '{rootPath}'");
        if (Directory.Exists(rootPath))
        {
            string msg = $"Project with name '{name}' already exists in '{directoryPath}'";
            logger.Error(msg);
            return Result<Project>.Failed(msg);
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
        await projectRepository.SaveAsync(project);

        foreach (ProjectVolume volume in volumes)
        {
            IVirtualVolume virtualVolume = new PhysicalVolume(volume.Id, volume.Scheme, volume.Name, volume.LogicalPath);
            volumeManager.MountVolume(virtualVolume);
        }

        logger.Info($"Project '{project.Name}' created.");

        Current = project;
        return project;
    }

    public async Task<Result<Project>> DeleteAsync(string projectFilePath)
    {
        AssertProjectFilePathState(projectFilePath);

        logger.Info($"Deleting project at path: '{projectFilePath}'");
        if (IsProjectFilePathCurrentProject(projectFilePath))
        {
            // If deleting the currently opened project then close it first
            await CloseAsync();
        }

        Project deletedProject = await projectRepository.LoadAsync(projectFilePath);
        if (IsInvalidState(deletedProject, projectFilePath))
        {
            string errorMsg = "The specified project file path is corrupted and cannot be safely deleted.";
            logger.Error(errorMsg);
            return Result<Project>.Failed(errorMsg);
        }

        string projectRootPath = Path.GetDirectoryName(projectFilePath)!;
        Directory.Delete(projectRootPath, recursive: true);

        logger.Info($"Project '{deletedProject.Name}' deleted.");
        return deletedProject;
    }

    public async Task<Result<Project>> OpenAsync(string projectFilePath)
    {
        AssertProjectFilePathState(projectFilePath);

        logger.Info($"Opening project at path: '{projectFilePath}'");
        if (IsProjectFilePathCurrentProject(projectFilePath))
        {
            // If opening currently opened project then return early
            string msg = $"Project '{Current.Name}' already opened.";
            logger.Warn(msg);
            return Result<Project>.Failed(msg);
        }

        if (IsOpen)
        {
            // If a project is opened, close it first
            await CloseAsync();
        }

        Project project = await projectRepository.LoadAsync(projectFilePath);

        // What happens if the project file name changed (e.g. test_project.pkproj -> new_project.pkproj) ?
        // Do we need to rename the provided file path to the saved one or rename the saved project to correspond the new project name ?
        if (IsInvalidState(project, projectFilePath))
        {
            /*
                TODO: Create a ProjectState to handle invalid states
                NOTE: If we arrive here it means the root path is not corresponding to the saved root path.
                      - The project root directory may have been moved or renamed
            */
            string msg = "The specified project file path is moved or corrupted and cannot be safely opened.";
            logger.Error(msg);
            return Result<Project>.Failed(msg);
        }

        foreach (ProjectVolume volume in project.Volumes)
        {
            if (!Directory.Exists(volume.LogicalPath))
            {
                logger.Warn($"Volume '{volume.Name}' wasn't found in the project root directory. Recreating.");
                Directory.CreateDirectory(volume.LogicalPath);
            }

            // Mounting the project volume to the virtual file system
            IVirtualVolume virtualVolume = new PhysicalVolume(volume.Id, volume.Scheme, volume.Name, volume.LogicalPath);
            volumeManager.MountVolume(virtualVolume);
        }

        logger.Info($"Project '{project.Name}' opened and configured.");
        Current = project;

        return project;
    }

    public async Task<Result<Project>> SaveAsync()
    {
        if (!IsOpen)
        {
            string msg = "Tried to save but no project is loaded.";
            logger.Warn(msg);
            return Result<Project>.Failed(msg);
        }

        logger.Info("Saving project");
        await projectRepository.SaveAsync(Current);
        logger.Info($"Project '{Current.Name}' saved.");

        return Current;
    }

    private bool IsProjectFilePathCurrentProject(string projectFilePath)
    {
        return Current != null && Path.Combine(Current.RootPath, $"{Current.Name}.pkproj") == projectFilePath;
    }

    private static bool IsInvalidState(Project project, string projectFilePath)
    {
        /*
            TODO: Create a ProjectState to handle invalid states
            NOTE: If we arrive here it means the root path is not corresponding to the saved root path.
                  - The project root directory may have been moved or renamed
                  - Other things I didn't thought about
        */
        string? projectRootPath = Path.GetDirectoryName(projectFilePath);
        return project.RootPath != projectRootPath;
    }

    private static void AssertProjectFilePathState(string projectFilePath)
    {
        ThrowHelper.AssertNotNullOrWhitespace(projectFilePath, "The project file path cannot be null or whitespace");
        ThrowHelper.Assert(File.Exists(projectFilePath), "The project file path must exist");
        ThrowHelper.Assert(Path.GetExtension(projectFilePath) == ".pkproj", "The project file extension must be '.pkproj'");
    }
}