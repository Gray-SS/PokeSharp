using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeCore.DependencyInjection.Abstractions;

public static class ServiceLocator
{
    public static IServiceResolver Services
    {
        get
        {
            EnsureInitialized();
            return _services!;
        }
    }

    public static bool IsInitialized => _services != null;

    private static IServiceResolver? _services;

    public static void Initialize(IServiceResolver services)
    {
        if (IsInitialized)
            throw new InvalidOperationException("ServiceLocator is already initialized. Cannot initialize the ServiceLocator twice.");

        _services = services;
    }

    public static void Cleanup()
    {
        _services = null;
    }

    public static object? GetService(Type serviceType)
    {
        EnsureInitialized();
        return _services!.GetService(serviceType);
    }

    public static object GetRequiredService(Type serviceType)
    {
        EnsureInitialized();
        return _services!.GetRequiredService(serviceType);
    }

    public static T? GetService<T>() where T : class
    {
        EnsureInitialized();
        return _services!.GetService<T>();
    }

    public static T GetRequiredService<T>() where T : class
    {
        EnsureInitialized();
        return _services!.GetRequiredService<T>();
    }

    public static IEnumerable<T> GetServices<T>() where T : class
    {
        EnsureInitialized();
        return _services!.GetServices<T>();
    }

    public static bool TryGetService(Type type, out object? service)
    {
        EnsureInitialized();
        return _services!.TryGetService(type, out service);
    }

    public static bool TryGetService<T>(out T? service) where T : class
    {
        EnsureInitialized();
        return _services!.TryGetService(out service);
    }

    private static void EnsureInitialized()
    {
        if (_services == null)
            throw new InvalidOperationException("ServiceLocator is not initialized. Make sure to call App.Run() first.");
    }
}