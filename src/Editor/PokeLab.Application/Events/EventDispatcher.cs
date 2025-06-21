using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeLab.Application.Events;

public sealed class EventDispatcher(
    IServiceResolver services
) : IEventDispatcher
{
    public async Task PublishAsync<TEvent>(TEvent @event)
        where TEvent : IEvent
    {
        IEnumerable<IEventHandler<TEvent>> handlers = services.GetServices<IEventHandler<TEvent>>();
        foreach (var handler in handlers)
        {
            await handler.HandleAsync(@event);
        }
    }
}