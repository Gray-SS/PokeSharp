using PokeLab.Domain;
using PokeLab.Application.Events;

namespace PokeLab.Application.ProjectManagement.Messages.Handlers;

public sealed class CreateProjectCommandHandler(
    IProjectService projectManager,
    IEventDispatcher eventDispatcher
) : ProjectCommandHandler<ProjectCommands.Create, ProjectEvents.Created, ProjectEvents.CreationFailed>(eventDispatcher)
{
    protected override Task<Result<Project>> ExecuteProjectOperationAsync(ProjectCommands.Create command)
        => projectManager.CreateAsync(command.Name, command.BasePath);

    protected override ProjectEvents.Created CreateSuccessEvent(Project project)
        => new(project);

    protected override ProjectEvents.CreationFailed CreateFailureEvent(string errorMessage)
        => new(errorMessage);
}
