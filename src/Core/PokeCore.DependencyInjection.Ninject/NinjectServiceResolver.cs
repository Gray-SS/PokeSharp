using Ninject;
using PokeCore.DependencyInjection.Abstractions;

namespace PokeCore.DependencyInjection.Ninject;

public sealed class NinjectServiceResolver : IServiceResolver, IDisposable
{
    public bool IsDisposed { get; private set; }

    private readonly IKernel _kernel;

    public NinjectServiceResolver(IKernel kernel)
    {
        _kernel = kernel;
    }

    public object? GetService(Type serviceType)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        return _kernel.GetService(serviceType);
    }

    public IEnumerable<object> GetServices(Type serviceType)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        return _kernel.GetAll(serviceType);
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