using System.Diagnostics.CodeAnalysis;
using Ninject;
using PokeSharp.Core.Exceptions;

namespace PokeSharp.Core;

// NOTE: Add dependency injection ?
public sealed class GameServiceProvider : IDisposable
{
    private bool _disposed;
    private readonly Dictionary<Type, object> _services = new();

    public void Add<T>(T instance) where T : class
    {
        Type serviceType = typeof(T);
        if (_services.ContainsKey(serviceType))
        {
            throw new EngineException($"""
                Service of type '{serviceType.Name}' is already registered.
                Make sure you have registered your service only once.
            """);
        }

        _services[serviceType] = instance;
    }

    public T Get<T>() where T : class
    {
        Type serviceType = typeof(T);
        if (!_services.TryGetValue(serviceType, out object? service))
        {
            throw new EngineException($"""
                Service of type '{serviceType.Name}' was not found.
                Make sure you've registered your service before using '{nameof(Get)}'
            """);
        }

        if (service is not T serviceT)
        {
            throw new EngineException($"""
                Service of type '{service.GetType().Name} is not assignable to '{serviceType.Name}'.
                The service has been found but couldn't be casted to '{serviceType.Name}'.
                Make sure you registered your service with the good generic type.
            """);
        }

        return serviceT;
    }

    public bool TryGet<T>([NotNullWhen(true)] out T? service) where T : class
    {
        service = null;

        Type serviceType = typeof(T);
        if (!_services.TryGetValue(serviceType, out object? result))
            return false;

        if (result is not T serviceT)
        {
            throw new EngineException($"""
                Service of type '{result.GetType().Name} is not assignable to '{serviceType.Name}'.
                The service has been found but couldn't be casted to '{serviceType.Name}'.
                Make sure you registered your service with the good generic type.
            """);
        }

        service = serviceT;
        return true;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            foreach (IDisposable service in _services.Values.OfType<IDisposable>())
            {
                service.Dispose();
            }

            _disposed = true;
        }
    }
}

public static class S
{
    public static object GetService(Type serviceType)
    {
        return Engine.Instance.Kernel.Get(serviceType);
    }

    public static T GetService<T>() where T : class
    {
        return Engine.Instance.Kernel.Get<T>();
    }

    public static bool TryGetService(Type serviceType, out object? service)
    {
        service = Engine.Instance.Kernel.TryGet(serviceType);
        return service != null;
    }

    public static bool TryGetService<T>(out T? service) where T : class
    {
        service = Engine.Instance.Kernel.TryGet<T>();
        return service != null;
    }
}