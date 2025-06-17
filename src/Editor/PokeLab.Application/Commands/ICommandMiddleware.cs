namespace PokeLab.Application.Commands;

public interface ICommandMiddleware
{
    Task InvokeAsync(ICommand command, Func<Task> next);
}