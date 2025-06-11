using System.Diagnostics.CodeAnalysis;
using Ninject;
using PokeCore.DependencyInjection.Abstractions;

namespace PokeCore.DependencyInjection.Ninject;

public sealed class NinjectServiceContainer : IServiceContainer
{
    private readonly IKernel _kernel;

    public NinjectServiceContainer(IKernel kernel)
    {
        _kernel = kernel;
    }

    public bool HasService(Type service)
    {
        return _kernel.GetBindings(service).Length != 0;
    }

    public bool HasService<T>() where T : class
    {
        return _kernel.GetBindings(typeof(T)).Length != 0;
    }

    public object GetService(Type service)
    {
        return _kernel.Get(service);
    }

    public T GetService<T>() where T : class
    {
        return _kernel.Get<T>();
    }

    public IEnumerable<T> GetServices<T>() where T : class
    {
        return _kernel.GetAll<T>();
    }

    public bool TryGetService(Type service, [NotNullWhen(true)] out object? implementation)
    {
        implementation = _kernel.TryGet(service);
        return implementation != null;
    }

    public bool TryGetService<T>([NotNullWhen(true)] out T? implementation) where T : class
    {
        implementation = _kernel.TryGet<T>();
        return implementation != null;
    }

    public bool TryGetServices<T>([NotNullWhen(true)] out IEnumerable<T>? implementation) where T : class
    {
        implementation = _kernel.GetAll<T>();
        return implementation != null;
    }
}