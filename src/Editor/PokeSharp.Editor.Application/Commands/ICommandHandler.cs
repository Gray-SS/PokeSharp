namespace PokeSharp.Editor.Application.Commands;

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    void Execute(TCommand command);
}