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
        IBindingInSyntax<object> binding = descriptor switch
        {
            ServiceDescriptor.TypeServiceDescriptor type => _kernel.Bind(descriptor.ServiceType).To(type.ImplementationType),
            ServiceDescriptor.FactoryServiceDescriptor factory => _kernel.Bind(descriptor.ServiceType).ToMethod(BuildProvider(factory.ImplementationFactory)),
            ServiceDescriptor.InstanceServiceDescriptor instance => _kernel.Bind(descriptor.ServiceType).ToConstant(instance.ImplementationInstance),
            _ => throw new NotImplementedException($"Binding service '{descriptor.ServiceType.Name}' failed. Service descriptor of type '{descriptor.GetType().Name}' aren't supported.")
        };

        switch (descriptor.Lifetime)
        {
            case ServiceLifetime.Singleton:
                binding.InSingletonScope();
                break;
            case ServiceLifetime.Transient:
                binding.InTransientScope();
                break;
            default:
                throw new NotImplementedException($"Binding service '{descriptor.ServiceType.Name}' failed. Lifetime of value '{descriptor.Lifetime}' aren't supported.");
        }

        return this;
    }

    private static Func<IContext, TService> BuildProvider<TService>(Func<IServiceResolver, TService> provider) where TService : class
    {
        return (context) => provider.Invoke(new NinjectServiceResolver(context.Kernel));
    }

    public IServiceResolver Build()
    {
        return new NinjectServiceResolver(_kernel);
    }
}