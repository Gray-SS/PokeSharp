using PokeCore.IO.Extensions;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;
using PokeCore.Common.Serializations;

namespace PokeCore.Common.Extensions;

public static class DependencyInjection
{
    public static IServiceCollections AddPokeCore(this IServiceCollections services)
    {
        services.AddSingleton<IDynamicTypeResolver, DynamicTypeResolver>();
        services.AddSingleton<IReflectionManager, ReflectionManager>();
        services.AddSingleton<IYamlSerializer, YamlSerializer>();

        services.AddPokeCoreIO();
        return services;
    }

    public static Type? GetUnderlyingServiceType(this IServiceResolver services, Type concreteType)
    {
        return concreteType.GetInterfaces().FirstOrDefault(x => services.HasService(x));
    }
}