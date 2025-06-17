using PokeLab.Application.Events;
using PokeLab.Domain;

namespace PokeLab.Application.ProjectManagement.Messages.Handlers;

public sealed class DeleteProjectCommandHandler(
    IProjectService projectManager,
    IEventDispatcher eventDispatcher
) : ProjectCommandHandler<ProjectCommands.Delete, ProjectEvents.Deleted, ProjectEvents.DeletionFailed>(eventDispatcher)
{
    protected override Task<Result<Project>> ExecuteProjectOperationAsync(ProjectCommands.Delete command)
        => projectManager.DeleteAsync(command.FileProjectPath);

    protected override ProjectEvents.Deleted CreateSuccessEvent(Project project)
        => new(project);

    protected override ProjectEvents.DeletionFailed CreateFailureEvent(string errorMessage)
        => new(errorMessage);
}
