using PokeLab.Application.Commands;

namespace PokeLab.Application.ProjectManagement;

public sealed record NewProjectCommand(string Name, string BasePath) : ICommand;

public sealed class NewProjectCommandHandler : ICommandHandler<NewProjectCommand>
{
    private readonly IProjectManager _projectManager;

    public NewProjectCommandHandler(IProjectManager projectManager)
    {
        _projectManager = projectManager;
    }

    public async Task ExecuteAsync(NewProjectCommand command)
    {
        await _projectManager.NewAsync(command.Name, command.BasePath);
    }
}

public sealed record OpenProjectCommand(string FileProjectPath) : ICommand;

public sealed class OpenProjectCommandHandler : ICommandHandler<OpenProjectCommand>
{
    private readonly IProjectManager _projectManager;

    public OpenProjectCommandHandler(IProjectManager projectManager)
    {
        _projectManager = projectManager;
    }

    public async Task ExecuteAsync(OpenProjectCommand command)
    {
        await _projectManager.OpenAsync(command.FileProjectPath);
    }
}
