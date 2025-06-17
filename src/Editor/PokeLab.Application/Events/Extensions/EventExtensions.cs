using System.Reflection;
using PokeCore.DependencyInjection.Abstractions;

namespace PokeLab.Application.Events.Extensions;

public static class EventExtensions
{
    public static IServiceCollections AddEvent(this IServiceCollections services)
    {
        services.AddTransient<IEventDispatcher, EventDispatcher>();
        return services;
    }

    public static IServiceCollections AddEventHandlers(this IServiceCollections services, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
        {
            assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        foreach (var type in assemblies.SelectMany(a => a.GetTypes()))
        {
            if (type.IsAbstract || type.IsInterface)
                continue;

            foreach (var interfaceType in type.GetInterfaces())
            {
                if (!interfaceType.IsGenericType)
                    continue;

                var genericDef = interfaceType.GetGenericTypeDefinition();

                if (genericDef == typeof(IEventHandler<>))
                {
                    services.AddTransient(interfaceType, type);
                }
            }
        }

        return services;
    }
}