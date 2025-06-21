using System.Reflection;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeLab.Application.Commands.Extensions;

public static class CommandExtensions
{
    public static IServiceCollections AddCommand(this IServiceCollections services)
    {
        services.AddTransient<ICommandDispatcher, CommandDispatcher>();
        return services;
    }

    public static IServiceCollections AddCommandMiddleware<TMiddleware>(this IServiceCollections services) where TMiddleware : class, ICommandMiddleware
    {
        services.AddSingleton<TMiddleware>();
        services.AddSingleton<ICommandMiddleware>(sc => sc.GetService<TMiddleware>());

        return services;
    }

    public static IServiceCollections AddCommandMiddleware<TMiddleware>(this IServiceCollections services, TMiddleware middleware) where TMiddleware : class, ICommandMiddleware
    {
        services.AddSingleton<TMiddleware>(middleware);
        services.AddSingleton<ICommandMiddleware>(sc => sc.GetService<TMiddleware>());

        return services;
    }

    public static IServiceCollections AddCommandHandlers(this IServiceCollections services, params Assembly[] assemblies)
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

                if (genericDef == typeof(ICommandHandler<>))
                {
                    services.AddTransient(interfaceType, type);
                }
            }
        }

        return services;
    }
}