using Spectre.Console.Cli;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeTools.Assets.CLI;

public sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollections _services;

    public TypeRegistrar(IServiceCollections services)
    {
        _services = services;
    }

    public ITypeResolver Build()
    {
        return new TypeResolver(_services.Build());
    }

    public void Register(Type service, Type implementation)
    {
        _services.AddSingleton(service, implementation);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        _services.AddSingleton(service, implementation);
    }

    public void RegisterLazy(Type service, Func<object> factory)
    {
        _services.AddSingleton(service, x => factory.Invoke());
    }
}

public sealed class TypeResolver : ITypeResolver, IDisposable
{
    private readonly IServiceResolver _services;

    public TypeResolver(IServiceResolver services)
    {
        _services = services;
        ServiceLocator.Initialize(services);
    }

    public object? Resolve(Type? type)
    {
        if (type == null)
            return null;

        return _services.GetService(type);
    }

    public void Dispose()
    {
        if (_services is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
