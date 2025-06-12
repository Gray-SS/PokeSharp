namespace PokeLab.Application.Commands;

public interface ICommandDispatcher
{
    Task ExecuteAsync<TCommand>(TCommand command)
        where TCommand : ICommand;
}