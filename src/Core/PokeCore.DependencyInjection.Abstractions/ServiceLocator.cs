namespace PokeCore.DependencyInjection.Abstractions;

public static class ServiceLocator
{
    public static IServiceContainer Services
    {
        get
        {
            EnsureInitialized();
            return _services!;
        }
    }

    public static bool IsInitialized => _services != null;

    private static IServiceContainer? _services;

    public static void Initialize(IServiceContainer services)
    {
        if (IsInitialized)
            throw new InvalidOperationException("ServiceLocator is already initialized. Cannot initialize the ServiceLocator twice.");

        _services = services;
    }

    public static void Cleanup()
    {
        _services = null;
    }

    public static object GetService(Type type)
    {
        EnsureInitialized();
        return _services!.GetService(type);
    }

    public static T GetService<T>() where T : class
    {
        EnsureInitialized();
        return _services!.GetService<T>();
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
        {
            throw new InvalidOperationException("ServiceLocator is not initialized. Make sure to call App.Run() first.");
        }
    }
}