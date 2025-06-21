using PokeCore.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PokeCore.DependencyInjection.Abstractions.Extensions;

public static class ServiceResolverExtensions
{
    public static object GetRequiredService(this IServiceResolver services, Type serviceType)
    {
        object? service = services.GetService(serviceType);
        ThrowHelper.AssertNotNull(service, $"Service of type '{serviceType.Name}' is not registered. Use '{nameof(TryGetService)}' if this is an expected behaviour.");

        return services;
    }

    public static T GetRequiredService<T>(this IServiceResolver services)
        where T : class
        => (T)services.GetRequiredService(typeof(T));

    public static T? GetService<T>(this IServiceResolver services)
        where T : class
    {
        if (services.TryGetService(out T? service))
            return service;

        return null;
    }

    public static IEnumerable<T> GetServices<T>(this IServiceResolver services)
    {
        object? service = services.GetService(typeof(T));

        if (service != null && service is IEnumerable<T> enumerable)
            return enumerable;

        return [];
    }

    public static bool HasService(this IServiceResolver services, Type serviceType)
    {
        object? service = services.GetService(serviceType);
        return service != null;
    }

    public static bool HasService<T>(this IServiceResolver services)
        where T : class
    {
        T? service = services.GetService<T>();
        return service != null;
    }

    public static bool TryGetService(this IServiceResolver services, Type serviceType, [NotNullWhen(true)] out object? service)
    {
        service = services.GetService(serviceType);
        return service != null;
    }

    public static bool TryGetService<T>(this IServiceResolver services, [NotNullWhen(true)] out T? service)
        where T : class
    {
        service = services.GetService(typeof(T)) as T;
        return service != null;
    }
}