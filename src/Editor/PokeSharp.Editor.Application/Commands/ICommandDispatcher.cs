using PokeSharp.Editor.Application.Commands.Async;

namespace PokeSharp.Editor.Application.Commands;

public interface ICommandDispatcher
{
    void Execute<TCommand>(TCommand command)
        where TCommand : ICommand;

    Task ExecuteAsync<TCommand>(TCommand command)
        where TCommand : ICommandAsync;
}