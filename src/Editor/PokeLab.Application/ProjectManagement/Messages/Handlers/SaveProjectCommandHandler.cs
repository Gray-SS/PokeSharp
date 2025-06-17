using PokeLab.Domain;
using PokeLab.Application.Events;

namespace PokeLab.Application.ProjectManagement.Messages.Handlers;

public sealed class SaveProjectCommandHandler(
    IProjectService projectManager,
    IEventDispatcher eventDispatcher
) : ProjectCommandHandler<ProjectCommands.Save, ProjectEvents.Saved, ProjectEvents.SaveFailed>(eventDispatcher)
{
    protected override Task<Result<Project>> ExecuteProjectOperationAsync(ProjectCommands.Save command)
        => projectManager.SaveAsync();

    protected override ProjectEvents.Saved CreateSuccessEvent(Project project)
        => new(project);

    protected override ProjectEvents.SaveFailed CreateFailureEvent(string errorMessage)
        => new(errorMessage);
}
