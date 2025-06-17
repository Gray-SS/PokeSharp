namespace PokeLab.Application.Commands;

public interface ICommandDispatcher
{
    Task SendAsync<TCommand>(TCommand command)
        where TCommand : ICommand;
}