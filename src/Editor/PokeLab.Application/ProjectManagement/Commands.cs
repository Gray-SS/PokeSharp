using PokeLab.Application.Commands.Async;

namespace PokeLab.Application.ProjectManagement;

public sealed record NewProjectCommand(string Name, string BasePath) : ICommandAsync;

public sealed class NewProjectCommandHandler : ICommandHandlerAsync<NewProjectCommand>
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