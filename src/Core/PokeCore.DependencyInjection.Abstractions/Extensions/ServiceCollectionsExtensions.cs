namespace PokeCore.DependencyInjection.Abstractions.Extensions;

public static class ServiceCollectionsExtensions
{
    public static IServiceCollections AddService(this IServiceCollections services, Type serviceType, Type implType, ServiceLifetime lifetime)
    {
        services.Add(ServiceDescriptor.FromType(serviceType, implType, lifetime));
        return services;
    }

    public static IServiceCollections AddService(this IServiceCollections services, Type serviceType, ServiceLifetime lifetime)
    {
        services.Add(ServiceDescriptor.FromType(serviceType, serviceType, lifetime));
        return services;
    }

    public static IServiceCollections AddService(this IServiceCollections services, Type serviceType, object instance, ServiceLifetime lifetime)
    {
        services.Add(ServiceDescriptor.FromInstance(serviceType, instance, lifetime));
        return services;
    }

    public static IServiceCollections AddService(this IServiceCollections services, Type serviceType, Func<IServiceResolver, object> provider, ServiceLifetime lifetime)
    {
        services.Add(ServiceDescriptor.FromFactory(serviceType, provider, lifetime));
        return services;
    }

    public static IServiceCollections AddService<TService, TImpl>(this IServiceCollections services, ServiceLifetime lifetime)
        where TService : class
        where TImpl : TService
    {
        services.Add(ServiceDescriptor.FromType(typeof(TService), typeof(TImpl), lifetime));
        return services;
    }

    public static IServiceCollections AddService<TService>(this IServiceCollections services, ServiceLifetime lifetime)
        where TService : class
    {
        services.Add(ServiceDescriptor.FromType(typeof(TService), typeof(TService), lifetime));
        return services;
    }

    public static IServiceCollections AddService<TService>(this IServiceCollections services, TService instance, ServiceLifetime lifetime)
        where TService : class
    {
        services.Add(ServiceDescriptor.FromInstance(typeof(TService), instance, lifetime));
        return services;
    }

    public static IServiceCollections AddService<TService>(this IServiceCollections services, Func<IServiceResolver, TService> provider, ServiceLifetime lifetime)
        where TService : class
    {
        services.Add(ServiceDescriptor.FromFactory(typeof(TService), provider, lifetime));
        return services;
    }

    #region Singleton Helpers

    public static IServiceCollections AddSingleton(this IServiceCollections services, Type serviceType, object instance)
        => services.AddService(serviceType, instance, ServiceLifetime.Singleton);

    public static IServiceCollections AddSingleton(this IServiceCollections services, Type serviceType, Type implType)
        => services.AddService(serviceType, implType, ServiceLifetime.Singleton);

    public static IServiceCollections AddSingleton(this IServiceCollections services, Type serviceType)
        => services.AddService(serviceType, ServiceLifetime.Singleton);

    public static IServiceCollections AddSingleton(this IServiceCollections services, Type serviceType, Func<IServiceResolver, object> provider)
        => services.AddService(serviceType, provider, ServiceLifetime.Singleton);

    public static IServiceCollections AddSingleton<TService>(this IServiceCollections services, TService instance)
        where TService : class
        => services.AddService(instance, ServiceLifetime.Singleton);

    public static IServiceCollections AddSingleton<TService, TImpl>(this IServiceCollections services)
        where TService : class
        where TImpl : TService
        => services.AddService<TService, TImpl>(ServiceLifetime.Singleton);

    public static IServiceCollections AddSingleton<TService>(this IServiceCollections services)
        where TService : class
        => services.AddService<TService>(ServiceLifetime.Singleton);

    public static IServiceCollections AddSingleton<TService>(this IServiceCollections services, Func<IServiceResolver, TService> provider)
        where TService : class
        => services.AddService<TService>(provider, ServiceLifetime.Singleton);

    #endregion // Singleton Helpers

    #region Transient Helpers
    public static IServiceCollections AddTransient(this IServiceCollections services, Type serviceType, object instance)
        => services.AddService(serviceType, instance, ServiceLifetime.Transient);

    public static IServiceCollections AddTransient(this IServiceCollections services, Type serviceType, Type implType)
        => services.AddService(serviceType, implType, ServiceLifetime.Transient);

    public static IServiceCollections AddTransient(this IServiceCollections services, Type serviceType)
        => services.AddService(serviceType, ServiceLifetime.Transient);

    public static IServiceCollections AddTransient(this IServiceCollections services, Type serviceType, Func<IServiceResolver, object> provider)
        => services.AddService(serviceType, provider, ServiceLifetime.Transient);

    public static IServiceCollections AddTransient<TService>(this IServiceCollections services, TService instance)
        where TService : class
        => services.AddService(instance, ServiceLifetime.Transient);

    public static IServiceCollections AddTransient<TService, TImpl>(this IServiceCollections services)
        where TService : class
        where TImpl : TService
        => services.AddService<TService, TImpl>(ServiceLifetime.Transient);

    public static IServiceCollections AddTransient<TService>(this IServiceCollections services)
        where TService : class
        => services.AddService<TService>(ServiceLifetime.Transient);

    public static IServiceCollections AddTransient<TService>(this IServiceCollections services, Func<IServiceResolver, TService> provider)
        where TService : class
        => services.AddService<TService>(provider, ServiceLifetime.Transient);

    #endregion // Transient Helper
}