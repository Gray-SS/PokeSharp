using PokeLab.Application.Commands.Async;

namespace PokeLab.Application.Commands;

public interface ICommandDispatcher
{
    void Execute<TCommand>(TCommand command)
        where TCommand : ICommand;

    Task ExecuteAsync<TCommand>(TCommand command)
        where TCommand : ICommandAsync;
}