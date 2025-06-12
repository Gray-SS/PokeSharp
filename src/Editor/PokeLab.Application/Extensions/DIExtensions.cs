using System.Reflection;
using PokeLab.Application.Commands;
using PokeCore.DependencyInjection.Abstractions;
using PokeLab.Application.ProjectManagement;

namespace PokeLab.Application.Extensions;

public static class DIExtensions
{
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

    public static IServiceCollections AddPokeLabApplication(this IServiceCollections services)
    {
        services.AddSingleton<IProjectManager, DefaultProjectManager>();

        services.AddTransient<ICommandDispatcher, CommandDispatcher>();
        services.AddCommandHandlers(typeof(DIExtensions).Assembly);

        return services;
    }
}