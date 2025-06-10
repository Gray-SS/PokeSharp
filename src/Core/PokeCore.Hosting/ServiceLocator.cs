using Ninject;

namespace PokeCore.Hosting;

public static class ServiceLocator
{
    private static IApp? _currentApp;

    internal static void Initialize(IApp app)
    {
        if (IsInitialized)
        {
            throw new InvalidOperationException($"ServiceLocator is already initialized with {_currentApp!.AppName}. Cannot initialize with {app.AppName}.");
        }

        _currentApp = app;
    }

    internal static void Cleanup()
    {
        _currentApp = null;
    }

    public static object GetService(Type type)
    {
        EnsureInitialized();
        EnsureSingleBinding(type);
        return _currentApp!.Kernel.Get(type);
    }

    public static T GetService<T>() where T : class
    {
        EnsureInitialized();
        EnsureSingleBinding(typeof(T));
        return _currentApp!.Kernel.Get<T>();
    }

    public static bool TryGetService(Type type, out object? service)
    {
        if (!IsInitialized || !HasSingleBinding(type))
        {
            service = null;
            return false;
        }

        try
        {
            service = _currentApp!.Kernel.Get(type);
            return true;
        }
        catch
        {
            service = null;
            return false;
        }
    }

    public static bool TryGetService<T>(out T? service) where T : class
    {
        if (!IsInitialized || !HasSingleBinding(typeof(T)))
        {
            service = null;
            return false;
        }

        try
        {
            service = _currentApp!.Kernel.Get<T>();
            return true;
        }
        catch
        {
            service = null;
            return false;
        }
    }

    public static T GetRequiredService<T>() where T : class
    {
        var service = GetService<T>();
        if (service == null)
        {
            throw new InvalidOperationException($"Required service '{typeof(T).Name}' returned null.");
        }
        return service;
    }

    public static bool IsServiceRegistered<T>() => IsServiceRegistered(typeof(T));

    public static bool IsServiceRegistered(Type type)
    {
        return IsInitialized && HasSingleBinding(type);
    }

    public static IApp CurrentApp
    {
        get
        {
            EnsureInitialized();
            return _currentApp!;
        }
    }

    public static bool IsInitialized => _currentApp != null;

    private static void EnsureInitialized()
    {
        if (_currentApp == null)
        {
            throw new InvalidOperationException("ServiceLocator is not initialized. Make sure to call App.Run() first.");
        }
    }

    private static void EnsureSingleBinding(Type type)
    {
        var bindings = _currentApp!.Kernel.GetBindings(type);

        if (bindings.Length == 0)
        {
            throw new InvalidOperationException($"No service registered for '{type.Name}'.");
        }

        if (bindings.Length > 1)
        {
            throw new InvalidOperationException($"Multiple bindings found for '{type.Name}'. Only one binding is allowed.");
        }
    }

    private static bool HasSingleBinding(Type type)
    {
        return IsInitialized && _currentApp!.Kernel.GetBindings(type).Length == 1;
    }
}