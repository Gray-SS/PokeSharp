using Ninject;
using Ninject.Activation;
using PokeCore.DependencyInjection.Abstractions;

namespace PokeCore.DependencyInjection.Ninject;

public sealed class NinjectServiceCollections : IServiceCollections
{
    private readonly StandardKernel _kernel;

    public NinjectServiceCollections()
    {
        _kernel = new StandardKernel();
    }

    public IServiceCollections AddSingleton<TService>(TService instance) where TService : class
    {
        _kernel.Bind<TService>().ToConstant(instance);
        return this;
    }

    public IServiceCollections AddSingleton<TService, TImpl>()
        where TService : class
        where TImpl : TService
    {
        _kernel.Bind<TService>().To<TImpl>().InSingletonScope();
        return this;
    }

    public IServiceCollections AddSingleton<TService>() where TService : class
    {
        _kernel.Bind<TService>().ToSelf().InSingletonScope();
        return this;
    }

    public IServiceCollections AddSingleton<TService>(Func<IServiceContainer, TService> provider) where TService : class
    {
        _kernel.Bind<TService>().ToMethod(BuildProvider(provider)).InSingletonScope();
        return this;
    }

    public IServiceCollections AddTransient<TService, TImpl>()
        where TService : class
        where TImpl : TService
    {
        _kernel.Bind<TService>().To<TImpl>().InTransientScope();
        return this;
    }

    public IServiceCollections AddTransient(Type service, Type implementation)
    {
        _kernel.Bind(service).To(implementation);
        return this;
    }

    public IServiceCollections AddTransient<TService>() where TService : class
    {
        _kernel.Bind<TService>().ToSelf().InTransientScope();
        return this;
    }

    public IServiceCollections AddTransient<TService>(Func<IServiceContainer, TService> provider) where TService : class
    {
        _kernel.Bind<TService>().ToMethod(BuildProvider(provider)).InTransientScope();
        return this;
    }

    private Func<IContext, TService> BuildProvider<TService>(Func<IServiceContainer, TService> provider) where TService : class
    {
        return (context) => provider.Invoke(new NinjectServiceContainer(_kernel));
    }

    public IServiceContainer Build()
    {
        return new NinjectServiceContainer(_kernel);
    }

    public void Dispose()
    {
        _kernel.Dispose();
    }
}