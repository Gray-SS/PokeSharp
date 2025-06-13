using Ninject;
using System.Diagnostics.CodeAnalysis;
using PokeCore.DependencyInjection.Abstractions;

namespace PokeCore.DependencyInjection.Ninject;

public sealed class NinjectServiceContainer : IServiceContainer, IDisposable
{
    public bool IsDisposed { get; private set; }

    private readonly IKernel _kernel;

    public NinjectServiceContainer(IKernel kernel)
    {
        _kernel = kernel;
    }

    public bool HasService(Type service)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        return _kernel.GetBindings(service).Length != 0;
    }

    public bool HasService<T>() where T : class
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        return _kernel.GetBindings(typeof(T)).Length != 0;
    }

    public object GetService(Type service)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        return _kernel.Get(service);
    }

    public T GetService<T>() where T : class
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        return _kernel.Get<T>();
    }

    public IEnumerable<T> GetServices<T>() where T : class
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        return _kernel.GetAll<T>();
    }

    public bool TryGetService(Type service, [NotNullWhen(true)] out object? implementation)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        implementation = _kernel.TryGet(service);
        return implementation != null;
    }

    public bool TryGetService<T>([NotNullWhen(true)] out T? implementation) where T : class
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        implementation = _kernel.TryGet<T>();
        return implementation != null;
    }

    public bool TryGetServices<T>([NotNullWhen(true)] out IEnumerable<T>? implementation) where T : class
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        implementation = _kernel.GetAll<T>();
        return implementation != null;
    }

    public void Dispose()
    {
        if (!IsDisposed)
        {
            _kernel.Dispose();
            IsDisposed = true;
        }
    }
}