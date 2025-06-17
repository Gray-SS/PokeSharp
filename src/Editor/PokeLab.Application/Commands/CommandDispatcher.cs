using PokeCore.DependencyInjection.Abstractions;

namespace PokeLab.Application.Commands;

public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceContainer _services;
    private readonly IReadOnlyCollection<ICommandMiddleware> _middlewares;

    public CommandDispatcher(IServiceContainer services, IEnumerable<ICommandMiddleware> middlewares)
    {
        _services = services;
        _middlewares = middlewares.ToList();
    }

    public async Task SendAsync<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        ICommandHandler<TCommand> handler = _services.GetService<ICommandHandler<TCommand>>();
        Task handlerFunc() => handler.HandleAsync(command);

        Func<Task> pipeline = _middlewares
            .Reverse()
            .Aggregate(handlerFunc, (next, middleware) =>
            {
                return () => middleware.InvokeAsync(command, next);
            });

        await pipeline.Invoke();
    }
}