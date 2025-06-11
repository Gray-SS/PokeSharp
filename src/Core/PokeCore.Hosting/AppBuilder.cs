
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Ninject;
using PokeCore.Diagnostics;
using PokeCore.Hosting.Abstractions;

namespace PokeCore.Hosting;

public sealed class AppBuilder : IAppBuilder
{
    private Func<IServiceCollections> _dependencyInjectionProviderFactory = () => new NinjectServiceCollections();

    public IAppBuilder UseDIProviderFactory(Func<IServiceCollections> factory)
    {
        ThrowHelper.AssertNotNull(factory, "The IoC provider factory MUST be not null");

        _dependencyInjectionProviderFactory = factory;
        return this;
    }

    public IServiceCollections Build()
    {
        if (_dependencyInjectionProviderFactory == null)
            throw new InvalidOperationException($"No dependency injection provider used. You must use the '{nameof(UseDIProviderFactory)}' in order to configure the app.");

        return _dependencyInjectionProviderFactory.Invoke();
    }
}