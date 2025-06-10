namespace PokeLab.Application.Commands.Async;

public interface ICommandHandlerAsync<TCommand> where TCommand : ICommandAsync
{
    Task ExecuteAsync(TCommand command);
}