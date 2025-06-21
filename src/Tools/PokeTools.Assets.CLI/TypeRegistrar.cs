using Spectre.Console.Cli;
using PokeCore.DependencyInjection.Abstractions;

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
    }

    public void RegisterInstance(Type service, object implementation)
    {
    }

    public void RegisterLazy(Type service, Func<object> factory)
    {
    }
}

public sealed class TypeResolver : ITypeResolver
{
    private readonly IServiceResolver _container;

    public TypeResolver(IServiceResolver container)
    {
        _container = container;
    }

    public object? Resolve(Type? type)
    {
        if (type == null)
            return null;

        return _container.GetService(type);
    }
}
