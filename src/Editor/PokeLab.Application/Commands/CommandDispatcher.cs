using Ninject;
using PokeLab.Application.Commands.Async;

namespace PokeLab.Application.Commands;

public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IKernel _kernel;

    public CommandDispatcher(IKernel kernel)
    {
        _kernel = kernel;
    }

    public void Execute<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        ICommandHandler<TCommand> handler = _kernel.Get<ICommandHandler<TCommand>>();
        handler.Execute(command);
    }

    public async Task ExecuteAsync<TCommand>(TCommand command)
        where TCommand : ICommandAsync
    {
        ICommandHandlerAsync<TCommand> handler = _kernel.Get<ICommandHandlerAsync<TCommand>>();
        await handler.ExecuteAsync(command);
    }
}