using PokeCore.IO.Extensions;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeCore.Common.Extensions;

public static class DependencyInjection
{
    public static IServiceCollections AddPokeCore(this IServiceCollections services)
    {
        services.AddSingleton<IDynamicTypeResolver, DynamicTypeResolver>();
        services.AddSingleton<IReflectionManager, ReflectionManager>();

        services.AddPokeCoreIO();
        return services;
    }

    public static Type? GetUnderlyingServiceType(this IServiceResolver services, Type concreteType)
    {
        return concreteType.GetInterfaces().FirstOrDefault(x => services.HasService(x));
    }
}