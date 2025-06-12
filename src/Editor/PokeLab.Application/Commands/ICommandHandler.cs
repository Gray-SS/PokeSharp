namespace PokeLab.Application.Commands;

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    Task ExecuteAsync(TCommand command);
}