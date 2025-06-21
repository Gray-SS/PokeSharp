using Ninject;
using Ninject.Syntax;
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

    public IServiceCollections Add(ServiceDescriptor descriptor)
    {
        IBindingInSyntax<object> binding;

        if (descriptor.ImplementationType != null) binding = _kernel.Bind(descriptor.ServiceType).To(descriptor.ImplementationType);
        else if (descriptor.ImplementationFactory != null) binding = _kernel.Bind(descriptor.ServiceType).ToMethod(BuildProvider(descriptor.ImplementationFactory));
        else if (descriptor.ImplementationInstance != null) binding = _kernel.Bind(descriptor.ServiceType).ToConstant(descriptor.ImplementationInstance);
        else throw new NotImplementedException($"Couldn't bind service '{descriptor.ServiceType.Name}' using Ninject.");

        switch (descriptor.Lifetime)
        {
            case ServiceLifetime.Singleton:
                binding.InSingletonScope();
                break;
            case ServiceLifetime.Transient:
                binding.InTransientScope();
                break;
            default:
                throw new NotImplementedException($"Couldn't bind service '{descriptor.ServiceType.Name}'. The lifetime '{descriptor.Lifetime}' isn't implemented in the Ninject IoC provider.");
        }

        return this;
    }

    private Func<IContext, TService> BuildProvider<TService>(Func<IServiceResolver, TService> provider) where TService : class
    {
        return (context) => provider.Invoke(new NinjectServiceResolver(_kernel));
    }

    public IServiceResolver Build()
    {
        return new NinjectServiceResolver(_kernel);
    }
}