using PokeCore.DependencyInjection.Abstractions;
using PokeLab.Application.Commands;

namespace PokeLab.Application.Extensions;

public static class DIExtensions
{
    public static IServiceCollections AddPokeLabApplication(this IServiceCollections services)
    {
        services.AddTransient<ICommandDispatcher, CommandDispatcher>();
        return services;
    }
}