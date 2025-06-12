using PokeCore.DependencyInjection.Abstractions;
using PokeLab.Application.Commands.Async;

namespace PokeLab.Application.Commands;

public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceContainer _services;

    public CommandDispatcher(IServiceContainer services)
    {
        _services = services;
    }

    public void Execute<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        ICommandHandler<TCommand> handler = _services.GetService<ICommandHandler<TCommand>>();
        handler.Execute(command);
    }

    public async Task ExecuteAsync<TCommand>(TCommand command)
        where TCommand : ICommandAsync
    {
        ICommandHandlerAsync<TCommand> handler = _services.GetService<ICommandHandlerAsync<TCommand>>();
        await handler.ExecuteAsync(command);
    }
}