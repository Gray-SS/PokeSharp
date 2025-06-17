using PokeCore.DependencyInjection.Abstractions;

namespace PokeLab.Application.Events;

public sealed class EventDispatcher(
    IServiceContainer services
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