using PokeLab.Domain;
using PokeLab.Application.Events;

namespace PokeLab.Application.ProjectManagement.Messages.Handlers;

public sealed class CloseProjectCommandHandler(
    IProjectService projectManager,
    IEventDispatcher eventDispatcher
) : ProjectCommandHandler<ProjectCommands.Close, ProjectEvents.Closed, ProjectEvents.CloseFailed>(eventDispatcher)
{
    protected override Task<Result<Project>> ExecuteProjectOperationAsync(ProjectCommands.Close command)
        => projectManager.CloseAsync();

    protected override ProjectEvents.Closed CreateSuccessEvent(Project project)
        => new(project);

    protected override ProjectEvents.CloseFailed CreateFailureEvent(string errorMessage)
        => new(errorMessage);
}