namespace PokeCore.DependencyInjection.Abstractions;

public interface IServiceCollections : IDisposable
{
    IServiceCollections AddSingleton<TService>(TService instance)
        where TService : class;

    IServiceCollections AddSingleton<TService, TImpl>()
        where TService : class
        where TImpl : TService;

    IServiceCollections AddSingleton<TService>()
        where TService : class;

    IServiceCollections AddTransient(Type service, Type implementation);

    IServiceCollections AddTransient<TService, TImpl>()
        where TService : class
        where TImpl : TService;

    IServiceCollections AddTransient<TService>()
        where TService : class;

    IServiceCollections AddSingleton<TService>(Func<IServiceContainer, TService> provider)
        where TService : class;

    IServiceCollections AddTransient<TService>(Func<IServiceContainer, TService> provider)
        where TService : class;

    IServiceContainer Build();
}