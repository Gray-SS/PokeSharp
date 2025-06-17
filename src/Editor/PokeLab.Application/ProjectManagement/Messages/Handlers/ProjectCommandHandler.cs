using PokeLab.Application.Commands;
using PokeLab.Application.Events;
using PokeLab.Domain;

namespace PokeLab.Application.ProjectManagement.Messages.Handlers;

public abstract class ProjectCommandHandler<TCommand, TSuccessEvent, TFailureEvent>(
    IEventDispatcher eventDispatcher
) : ICommandHandler<TCommand>
    where TCommand : ICommand
    where TSuccessEvent : IEvent
    where TFailureEvent : IEvent
{
    public async Task HandleAsync(TCommand command)
    {
        Result<Project> result = await ExecuteProjectOperationAsync(command);
        if (result.IsSuccess)
        {
            TSuccessEvent successEvent = CreateSuccessEvent(result.Value);
            await eventDispatcher.PublishAsync(successEvent);
        }
        else
        {
            TFailureEvent failureEvent = CreateFailureEvent(result.ErrorMessage!);
            await eventDispatcher.PublishAsync(failureEvent);
        }
    }

    protected abstract Task<Result<Project>> ExecuteProjectOperationAsync(TCommand command);
    protected abstract TSuccessEvent CreateSuccessEvent(Project project);
    protected abstract TFailureEvent CreateFailureEvent(string errorMessage);
}
