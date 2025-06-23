namespace PokeCore.DependencyInjection.Abstractions;

public abstract record class ServiceDescriptor(ServiceLifetime Lifetime, Type ServiceType)
{
    public sealed record class TypeServiceDescriptor(ServiceLifetime Lifetime, Type ServiceType, Type ImplementationType) : ServiceDescriptor(Lifetime, ServiceType);
    public sealed record class InstanceServiceDescriptor(ServiceLifetime Lifetime, Type ServiceType, object ImplementationInstance) : ServiceDescriptor(Lifetime, ServiceType);
    public sealed record class FactoryServiceDescriptor(ServiceLifetime Lifetime, Type ServiceType, Func<IServiceResolver, object> ImplementationFactory) : ServiceDescriptor(Lifetime, ServiceType);

    public static ServiceDescriptor FromType(Type serviceType, Type implType, ServiceLifetime lifetime)
    {
        return new TypeServiceDescriptor(lifetime, serviceType, implType);
    }

    public static ServiceDescriptor FromFactory(Type serviceType, Func<IServiceResolver, object> factory, ServiceLifetime lifetime)
    {
        return new FactoryServiceDescriptor(lifetime, serviceType, factory);
    }

    public static ServiceDescriptor FromInstance(Type serviceType, object instance, ServiceLifetime lifetime)
    {
        return new InstanceServiceDescriptor(lifetime, serviceType, instance);
    }
}