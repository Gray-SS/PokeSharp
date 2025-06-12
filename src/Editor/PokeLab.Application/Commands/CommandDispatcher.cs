using PokeCore.DependencyInjection.Abstractions;

namespace PokeLab.Application.Commands;

public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceContainer _services;

    public CommandDispatcher(IServiceContainer services)
    {
        _services = services;
    }

    public async Task ExecuteAsync<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        ICommandHandler<TCommand> handler = _services.GetService<ICommandHandler<TCommand>>();
        await handler.ExecuteAsync(command);
    }
}