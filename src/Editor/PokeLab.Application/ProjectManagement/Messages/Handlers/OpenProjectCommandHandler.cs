using PokeLab.Domain;
using PokeLab.Application.Events;

namespace PokeLab.Application.ProjectManagement.Messages.Handlers;

public sealed class OpenProjectCommandHandler(
    IProjectService projectManager,
    IEventDispatcher eventDispatcher
) : ProjectCommandHandler<ProjectCommands.Open, ProjectEvents.Opened, ProjectEvents.OpenFailed>(eventDispatcher)
{
    protected override Task<Result<Project>> ExecuteProjectOperationAsync(ProjectCommands.Open command)
        => projectManager.OpenAsync(command.FileProjectPath);

    protected override ProjectEvents.Opened CreateSuccessEvent(Project project)
        => new(project);

    protected override ProjectEvents.OpenFailed CreateFailureEvent(string errorMessage)
        => new(errorMessage);
}
