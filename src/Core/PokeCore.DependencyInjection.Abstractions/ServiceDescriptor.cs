namespace PokeCore.DependencyInjection.Abstractions;

public sealed class ServiceDescriptor
{
    public ServiceLifetime Lifetime { get; }
    public Type ServiceType { get; }
    public Type? ImplementationType { get; }
    public object? ImplementationInstance { get; }
    public Func<IServiceResolver, object>? ImplementationFactory { get; }

    public ServiceDescriptor(Type serviceType, Type implType, ServiceLifetime lifetime)
    {
        ServiceType = serviceType;
        ImplementationType = implType;
        Lifetime = lifetime;
    }

    public ServiceDescriptor(Type serviceType, object implInstance, ServiceLifetime lifetime)
    {
        ServiceType = serviceType;
        ImplementationInstance = implInstance;
        Lifetime = lifetime;
    }

    public ServiceDescriptor(Type serviceType, Func<IServiceProvider, object> implFactory, ServiceLifetime lifetime)
    {
        ServiceType = serviceType;
        ImplementationFactory = implFactory;
        Lifetime = lifetime;
    }
}